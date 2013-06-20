using PropertyChanged;

namespace HierarchyBeforeAfterAndSimple
{
    public class ClassChild : ClassBase
    {

        public string Property1 { get; set; }

        [DoNotNotify]
        public bool BeforeAfterCalled { get; set; }

        public void OnPropertyChanged(string propertyName, object before, object after)
        {
            BeforeAfterCalled = true;
            OnPropertyChanged(propertyName);
        }

    }
}