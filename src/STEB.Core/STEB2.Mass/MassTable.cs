using System.Collections.Generic;
using STEB2.Geometry;

namespace STEB2.Mass;

public class MassTable
{
	public string Name = "";

	public float Mass;

	public p3d GPosition = default;

	public p3d Mp = default;

	public List<MassCategory> lstMassCategories = new List<MassCategory>();
}
