﻿namespace AssemblyWithBaseInDifferentModule.Hierarchy
{
    using AssemblyWithBase.StaticEquals;

    public class ChildClass : StaticEquals
    {
        private string property1;
        public string Property1
        {
            get => property1;
            set
            {
                property1 = value;
                Property2 = new BaseClass();
            }
        }

        public BaseClass Property2 { get; set; }
    }
}