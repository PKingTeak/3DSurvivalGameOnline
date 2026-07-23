public abstract class UnitPart
{
    public PartObjectData Data { get; private set; }
    protected UnitContext Context { get; private set; }  //컨텍스트 만들어서 부모 클래스에게 부담없이 추상적으로 생성

    public PartSlot Slot => Data.partSlot;
    public string PartName => Data.part_name;

    public bool IsInitialized { get; private set; }

    public void InitPart(PartObjectData data, UnitContext context)
    {
        if (data == null)
        {
            throw new System.ArgumentNullException(nameof(data));
        }

        if (context == null)
        {
            throw new System.ArgumentNullException(nameof(context));
        }
        Context = context;


        Data = data;
        IsInitialized = true;

        OnInitialized();
    }

    protected virtual void OnInitialized()
    {
    }

    public virtual void OnEquip()
    {
    }

    public virtual void OnUnequip()
    {
    }
}