namespace Archview.Model
{
    public abstract record Resource
    {
        public string Id
        {
            get { return Name.ToLower().Replace(' ', '-'); }
            set { }
        } 
        public string Name { get; init; }

        public Resource()
        {
        }
    }
}
