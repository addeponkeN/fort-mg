namespace Fort.MG.Example;

public class GuyData
{
    public string Name { get; set; }
    public string Version { get; set; }
    public bool IsTrue { get; set; }
    public int Age { get; set; }
    public Address Address { get; set; }
}

public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public int Zip { get; set; }
}