using STEB2.Geometry;

namespace STEB2.Stations;

public class Station
{
	public float X;

	public MultipleContur StationContur;

	public Station(MultipleContur StationContur, float X)
	{
		this.StationContur = StationContur;
		this.X = X;
	}

	public p2d[] GetSledOnDP()
	{
		p2d[] array = new p2d[2];
		bool flag = false;
		Contur[] arrayConturs = StationContur.arrayConturs;
		foreach (Contur contur in arrayConturs)
		{
			if (!flag)
			{
				p2d[] boundRect = Mathematics.GetBoundRect(contur.arrayPoints);
				array[0].X = X;
				array[0].Y = boundRect[0].Y;
				array[1].X = X;
				array[1].Y = boundRect[1].Y;
				flag = true;
			}
			else
			{
				p2d[] boundRect2 = Mathematics.GetBoundRect(contur.arrayPoints);
				array[0].X = X;
				array[0].Y = ((array[0].Y > boundRect2[0].Y) ? boundRect2[0].Y : array[0].Y);
				array[1].X = X;
				array[1].Y = ((array[1].Y < boundRect2[1].Y) ? boundRect2[1].Y : array[1].Y);
			}
		}
		return array;
	}

	public p2d[] GetSledOnTop()
	{
		p2d[] array = new p2d[2];
		bool flag = false;
		Contur[] arrayConturs = StationContur.arrayConturs;
		foreach (Contur contur in arrayConturs)
		{
			if (!flag)
			{
				p2d[] boundRect = Mathematics.GetBoundRect(contur.arrayPoints);
				array[0].X = X;
				array[0].Y = boundRect[0].X;
				array[1].X = X;
				array[1].Y = boundRect[2].X;
				flag = true;
			}
			else
			{
				p2d[] boundRect2 = Mathematics.GetBoundRect(contur.arrayPoints);
				array[0].X = X;
				array[0].Y = ((array[0].Y > boundRect2[0].X) ? boundRect2[0].X : array[0].Y);
				array[1].X = X;
				array[1].Y = ((array[1].Y < boundRect2[2].X) ? boundRect2[2].X : array[1].Y);
			}
		}
		return array;
	}
}
