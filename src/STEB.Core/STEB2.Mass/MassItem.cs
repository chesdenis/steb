using STEB2.Geometry;

namespace STEB2.Mass;

public class MassItem
{
	public string id;

	public string Name;

	public string MassTableName;

	public string CatName;

	public p3d GPosition = default;

	public p3d Mp = default;

	public float Mass;

	public float Area;

	public float Thickness;

	public float Density;

	public MassItem()
	{
	}

	public MassItem(string MassTableName, string Name, string CatName, p3d GPosition, float Mass, float Area, float Thickness, float Density)
	{
		id = MassTableName + "." + CatName + "." + Name;
		this.MassTableName = MassTableName;
		this.Name = Name;
		this.CatName = CatName;
		this.GPosition = GPosition;
		this.Mass = Mass;
		this.Area = Area;
		this.Thickness = Thickness;
		this.Density = Density;
	}
}
