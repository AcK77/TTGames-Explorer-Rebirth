using System.ComponentModel;

namespace FastColoredTextBoxNS
{
    ///
    /// These classes are required for correct data binding to Text property of FastColoredTextbox
    /// 
    class FCTBDescriptionProvider(Type type) : TypeDescriptionProvider(GetDefaultTypeProvider(type))
    {
        private static TypeDescriptionProvider GetDefaultTypeProvider(Type type)
        {
            return TypeDescriptor.GetProvider(type);
        }

        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            ICustomTypeDescriptor defaultDescriptor = base.GetTypeDescriptor(objectType, instance);

            return new FCTBTypeDescriptor(defaultDescriptor, instance);
        }
    }

    class FCTBTypeDescriptor(ICustomTypeDescriptor parent, object instance) : CustomTypeDescriptor(parent)
    {
        private readonly object _instance = instance;

        public override string GetComponentName()
        {
            return (_instance as Control)?.Name;
        }

        public override EventDescriptorCollection GetEvents()
        {
            var coll = base.GetEvents();
            var list = new EventDescriptor[coll.Count];

            for (int i = 0; i < coll.Count; i++)
            {
                // Instead of TextChanged slip BindingTextChanged for binding.
                if (coll[i].Name == "TextChanged")
                {
                    list[i] = new FooTextChangedDescriptor(coll[i]);
                }
                else
                {
                    list[i] = coll[i];
                }
            }

            return new EventDescriptorCollection(list);
        }
    }

    class FooTextChangedDescriptor(MemberDescriptor desc) : EventDescriptor(desc)
    {
        public override void AddEventHandler(object component, Delegate value)
        {
            (component as FastColoredTextBox).BindingTextChanged += value as EventHandler;
        }

        public override Type ComponentType
        {
            get { return typeof(FastColoredTextBox); }
        }

        public override Type EventType
        {
            get { return typeof(EventHandler); }
        }

        public override bool IsMulticast
        {
            get { return true; }
        }

        public override void RemoveEventHandler(object component, Delegate value)
        {
            (component as FastColoredTextBox).BindingTextChanged -= value as EventHandler;
        }
    }
}