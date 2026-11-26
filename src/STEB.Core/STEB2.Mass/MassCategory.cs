using System.Collections.Generic;
using STEB2.Geometry;

namespace STEB2.Mass;

public class MassCategory
{
	public string Name = "";

	public float Mass;

	public p3d GPosition = default;

	public p3d Mp = default;

	public List<MassItem> lstMassItems = new List<MassItem>();
}
