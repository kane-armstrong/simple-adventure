namespace PetDoctor.API.Application;

public readonly struct Unit
{
    private static readonly Unit _value = new();
    public static ref readonly Unit Value => ref _value;
}