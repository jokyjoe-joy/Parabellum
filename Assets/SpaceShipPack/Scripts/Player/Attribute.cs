[System.Serializable]
public class Attribute
{
    [System.NonSerialized] public ShipController parent;
    public Attributes type;
    public ModifiableInt value;

    public void SetParent(ShipController _parent)
    {
        parent = _parent;
        value = new ModifiableInt(AttributeModified);
    }

    public void AttributeModified()
    {
        parent.AttributeModified(this);
    }
}