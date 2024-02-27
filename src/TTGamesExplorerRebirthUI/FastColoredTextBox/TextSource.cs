using System.Collections;
using System.Text;

namespace FastColoredTextBoxNS
{
    /// <summary>
    /// This class contains the source text (chars and styles).
    /// It stores a text lines, the manager of commands, undo/redo stack, styles.
    /// </summary>
    public class TextSource: IList<Line>, IDisposable
    {
        private readonly protected List<Line> _lines = [];
        private protected LinesAccessor _linesAccessor;
        private int _lastLineUniqueId;
        private FastColoredTextBox _currentTB;
        private bool _disposedValue;

        /// <summary>
        /// Styles
        /// </summary>
        public readonly Style[] Styles;

        /// <summary>
        /// Occurs when line was inserted/added
        /// </summary>
        public event EventHandler<LineInsertedEventArgs> LineInserted;

        /// <summary>
        /// Occurs when line was removed
        /// </summary>
        public event EventHandler<LineRemovedEventArgs> LineRemoved;

        /// <summary>
        /// Occurs when text was changed
        /// </summary>
        public event EventHandler<TextChangedEventArgs> TextChanged;

        /// <summary>
        /// Occurs when recalc is needed
        /// </summary>
        public event EventHandler<TextChangedEventArgs> RecalcNeeded;

        /// <summary>
        /// Occurs when recalc wordwrap is needed
        /// </summary>
        public event EventHandler<TextChangedEventArgs> RecalcWordWrap;

        /// <summary>
        /// Occurs before text changing
        /// </summary>
        public event EventHandler<TextChangingEventArgs> TextChanging;

        /// <summary>
        /// Occurs after CurrentTB was changed
        /// </summary>
        public event EventHandler CurrentTBChanged;

        /// <summary>
        /// Current focused FastColoredTextBox
        /// </summary>
        public FastColoredTextBox CurrentTB {
            get => _currentTB;
            set
            {
                if (_currentTB == value)
                {
                    return;
                }

                _currentTB = value;

                OnCurrentTBChanged(); 
            }
        }

        public CommandManager Manager { get; set; }

        /// <summary>
        /// Default text style
        /// This style is using when no one other TextStyle is not defined in Char.style
        /// </summary>
        public TextStyle DefaultStyle { get; set; }

        public TextSource(FastColoredTextBox currentTB)
        {
            CurrentTB = currentTB;
            _linesAccessor = new LinesAccessor(this);
            Manager = new CommandManager(this);

            if (Enum.GetUnderlyingType(typeof(StyleIndex)) == typeof(uint))
            {
                Styles = new Style[32];
            }
            else
            {
                Styles = new Style[16];
            }

            InitDefaultStyle();
        }

        public virtual void InitDefaultStyle()
        {
            DefaultStyle = new TextStyle(null, null, FontStyle.Regular);
        }

        public virtual void ClearIsChanged()
        {
            foreach (var line in _lines)
            {
                line.IsChanged = false;
            }
        }

        public virtual Line CreateLine()
        {
            return new Line(GenerateUniqueLineId());
        }

        private void OnCurrentTBChanged()
        {
            CurrentTBChanged?.Invoke(this, EventArgs.Empty);
        }

        public virtual Line this[int i]
        {
            get => _lines[i];
            set
            {
                throw new NotImplementedException();
            }
        }

        public virtual bool IsLineLoaded(int iLine)
        {
            return _lines[iLine] != null;
        }

        /// <summary>
        /// Text lines
        /// </summary>
        public virtual IList<string> GetLines()
        {
            return _linesAccessor;
        }

        public IEnumerator<Line> GetEnumerator()
        {
            return _lines.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (_lines  as IEnumerator);
        }

        public virtual int BinarySearch(Line item, IComparer<Line> comparer)
        {
            return _lines.BinarySearch(item, comparer);
        }

        public virtual int GenerateUniqueLineId()
        {
            return _lastLineUniqueId++;
        }

        public virtual void InsertLine(int index, Line line)
        {
            _lines.Insert(index, line);

            OnLineInserted(index);
        }

        public virtual void OnLineInserted(int index)
        {
            OnLineInserted(index, 1);
        }

        public virtual void OnLineInserted(int index, int count)
        {
            LineInserted?.Invoke(this, new LineInsertedEventArgs(index, count));
        }

        public virtual void RemoveLine(int index)
        {
            RemoveLine(index, 1);
        }

        public virtual bool IsNeedBuildRemovedLineIds
        {
            get { return LineRemoved != null; }
        }

        public virtual void RemoveLine(int index, int count)
        {
            List<int> removedLineIds = [];

            if (count > 0)
            {
                if (IsNeedBuildRemovedLineIds)
                {
                    for (int i = 0; i < count; i++)
                    {
                        removedLineIds.Add(this[index + i].UniqueId);
                    }
                }
            }

            _lines.RemoveRange(index, count);

            OnLineRemoved(index, count, removedLineIds);
        }

        public virtual void OnLineRemoved(int index, int count, List<int> removedLineIds)
        {
            if (count > 0)
            {
                LineRemoved?.Invoke(this, new LineRemovedEventArgs(index, count, removedLineIds));
            }
        }

        public virtual void OnTextChanged(int fromLine, int toLine)
        {
            TextChanged?.Invoke(this, new TextChangedEventArgs(Math.Min(fromLine, toLine), Math.Max(fromLine, toLine)));
        }

        public class TextChangedEventArgs(int iFromLine, int iToLine) : EventArgs
        {
            public int IFromLine = iFromLine;
            public int IToLine = iToLine;
        }

        public virtual int IndexOf(Line item)
        {
            return _lines.IndexOf(item);
        }

        public virtual void Insert(int index, Line item)
        {
            InsertLine(index, item);
        }

        public virtual void RemoveAt(int index)
        {
            RemoveLine(index);
        }

        public virtual void Add(Line item)
        {
            InsertLine(Count, item);
        }

        public virtual void Clear()
        {
            RemoveLine(0, Count);
        }

        public virtual bool Contains(Line item)
        {
            return _lines.Contains(item);
        }

        public virtual void CopyTo(Line[] array, int arrayIndex)
        {
            _lines.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Lines count
        /// </summary>
        public virtual int Count => _lines.Count;

        public virtual bool IsReadOnly => false;

        public virtual bool Remove(Line item)
        {
            int i = IndexOf(item);
            if (i >= 0)
            {
                RemoveLine(i);

                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual void NeedRecalc(TextChangedEventArgs args)
        {
            RecalcNeeded?.Invoke(this, args);
        }

        public virtual void OnRecalcWordWrap(TextChangedEventArgs args)
        {
            RecalcWordWrap?.Invoke(this, args);
        }

        public virtual void OnTextChanging()
        {
            string temp = null;

            OnTextChanging(ref temp);
        }

        public virtual void OnTextChanging(ref string text)
        {
            if (TextChanging != null)
            {
                var args = new TextChangingEventArgs()
                {
                    InsertingText = text,
                };

                TextChanging(this, args);

                text = args.InsertingText;

                if (args.Cancel)
                {
                    text = string.Empty;
                }
            };
        }

        public virtual int GetLineLength(int i)
        {
            return _lines[i].Count;
        }

        public virtual bool LineHasFoldingStartMarker(int iLine)
        {
            return !string.IsNullOrEmpty(_lines[iLine].FoldingStartMarker);
        }

        public virtual bool LineHasFoldingEndMarker(int iLine)
        {
            return !string.IsNullOrEmpty(_lines[iLine].FoldingEndMarker);
        }

        public virtual void SaveToFile(string fileName, Encoding enc)
        {
            using StreamWriter sw = new(fileName, false, enc);

            for (int i = 0; i < Count - 1; i++)
            {
                sw.WriteLine(_lines[i].Text);
            }

            sw.Write(_lines[Count - 1].Text);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                _disposedValue = true;
            }
        }

        public virtual void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}