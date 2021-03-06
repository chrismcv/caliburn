namespace Tests.Caliburn.Fakes.Model
{
    public class MyPresenter
    {
        public object Model { get; set; }
        public MyModel TypedModel { get; set; }
    }

    public class MyModel
    {
        public string MyProperty { get; set; }
        public string MyProperty_HasUnderscore { get; set; }

        public object SubModel { get; set; }
        public MySubModel TypedSubModel { get; set; }
    }

    public class MySubModel
    {
        public int MySubProperty { get; set; }
        public object Parent { get; set; }
    }
}