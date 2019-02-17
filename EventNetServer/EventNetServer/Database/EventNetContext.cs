namespace EventNetServer.Database
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class EventNetContext : DbContext
    {
        public EventNetContext()
            : base("name=EventNetContext")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }

        public virtual DbSet<MyEvent> Events { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Beacon> Beacons { get; set; }
    }
}