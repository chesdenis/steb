using STEB2.Geometry;

namespace STEB2.Containers;

public class Container
{
	public string Name;

	public float l;

	public float b;

	public float h;

	public p3d pos;

	public float Mass;

	public Container(string Name, float l, float b, float h, p3d pos, float Mass)
	{
		this.Name = Name;
		this.l = l;
		this.b = b;
		this.h = h;
		this.pos = new p3d(pos.X, pos.Y, pos.Z);
		this.Mass = Mass;
	}
}
