using System.Collections.Generic;
using STEB2.Geometry;

namespace STEB2.Stability;

public class LongitudinalDynStability : StabilityCurve
{
	public LongitudinalDynStability(float DSPL, string Name, List<p2d> Points)
		: base(DSPL, Name, Points)
	{
	}
}
