namespace DTVS_TestRig
{
    public class TestM
    {
        public TestM()
        {
            this.test = new TestPlansM();
        }
        public TestPlansM test { get; set; }
        public string error { get; set; }
    }
}