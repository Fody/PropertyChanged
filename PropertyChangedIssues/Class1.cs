namespace PropertyChangedIssues
{
    partial class Class1
    {
        public int CalledProperty1OnChangedMethod { get; private set; }
        public int CalledIdOnChangedMethod { get; private set; }

        partial void OnProperty1Changed()
        {
            CalledProperty1OnChangedMethod++;
        }

        partial void OnIdChanged()
        {
            CalledIdOnChangedMethod++;
        }
    }
}
