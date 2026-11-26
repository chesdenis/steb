using System.Collections.Generic;
using STEB2.Geometry;
using STEB2.Stations;
using STEB2.Volume;

namespace STEB2.Cisterns;

public class Cistern
{
	private float _maxVolume;

	private float _maxMass;

	private float _CalcVolume;

	private float _CalcMass;

	private float _waterLevel;

	public string Name;

	private float _l;

	private float _b;

	private float _h;

	private float _x0;

	private float _y0;

	private float _z0;

	private float _x1;

	private float _y1;

	private float _z1;

	private float _percentOfVolume;

	private float _volumeDensity;

	public Waterlanding wtrl;

	public List<Station> Stations;

	private string _TextDecriptionOfStation;

	public float maxVolume => _maxVolume;

	public float maxMass => _maxMass;

	public float CalcVolume
	{
		get
		{
			return _CalcVolume;
		}
		set
		{
			wtrl = new Waterlanding_Volume_Heel_Trim(value, 0f, 0f, Stations, 0.001f, volumeDensity);
			_maxMass = STEB2.Volume.Tools.GetMaxVolume(Stations) * volumeDensity;
			_maxVolume = STEB2.Volume.Tools.GetMaxVolume(Stations);
			_CalcMass = value * volumeDensity;
			_CalcVolume = value;
			_percentOfVolume = CalcVolume / maxVolume * 100f;
			_volumeDensity = volumeDensity;
			_waterLevel = wtrl.wlPosition.Z - z0;
		}
	}

	public float CalcMass
	{
		get
		{
			return _CalcMass;
		}
		set
		{
			wtrl = new Waterlanding_Volume_Heel_Trim(value / volumeDensity, 0f, 0f, Stations, 0.001f, volumeDensity);
			_maxMass = STEB2.Volume.Tools.GetMaxVolume(Stations) * volumeDensity;
			_maxVolume = STEB2.Volume.Tools.GetMaxVolume(Stations);
			_CalcMass = value;
			_CalcVolume = value / volumeDensity;
			_percentOfVolume = CalcVolume / maxVolume * 100f;
			_volumeDensity = volumeDensity;
			_waterLevel = wtrl.wlPosition.Z - z0;
		}
	}

	public float waterLevel
	{
		get
		{
			return _waterLevel;
		}
		set
		{
			wtrl = new Waterlanding_WaterlinePosition(Stations, 0f, 0f, new p3d(0f, 0f, z0 + value), volumeDensity);
			_maxMass = STEB2.Volume.Tools.GetMaxVolume(Stations) * volumeDensity;
			_maxVolume = STEB2.Volume.Tools.GetMaxVolume(Stations);
			_CalcMass = wtrl.V * volumeDensity;
			_CalcVolume = wtrl.V;
			_percentOfVolume = CalcVolume / maxVolume * 100f;
			_volumeDensity = volumeDensity;
			_waterLevel = wtrl.wlPosition.Z - z0;
		}
	}

	public float l
	{
		get
		{
			return _l;
		}
		set
		{
			Cistern cistern = new Cistern(Name, TextDecriptionOfStation, new p3d(x0, y0, z0), new p3d(x0 + value, y1, z1), volumeDensity);
			cistern.percentOfVolume = percentOfVolume;
			Stations = cistern.Stations;
			_TextDecriptionOfStation = cistern.TextDecriptionOfStation;
			_l = cistern.l;
			_b = cistern.b;
			_h = cistern.h;
			Name = cistern.Name;
			_x0 = cistern.x0;
			_x1 = cistern.x1;
			_y0 = cistern.y0;
			_y1 = cistern.y1;
			_z0 = cistern.z0;
			_z1 = cistern.z1;
			_maxMass = cistern.maxMass;
			_maxVolume = cistern.maxVolume;
			_CalcMass = cistern.CalcMass;
			_CalcVolume = cistern.CalcVolume;
			_percentOfVolume = cistern.percentOfVolume;
			_volumeDensity = cistern.volumeDensity;
			_waterLevel = cistern.waterLevel;
			wtrl = cistern.wtrl;
		}
	}

	public float b
	{
		get
		{
			return _b;
		}
		set
		{
			Cistern cistern = new Cistern(Name, TextDecriptionOfStation, new p3d(x0, y0, z0), new p3d(x1, y0 + value, z1), volumeDensity);
			cistern.percentOfVolume = percentOfVolume;
			Stations = cistern.Stations;
			_TextDecriptionOfStation = cistern.TextDecriptionOfStation;
			_l = cistern.l;
			_b = cistern.b;
			_h = cistern.h;
			Name = cistern.Name;
			_x0 = cistern.x0;
			_x1 = cistern.x1;
			_y0 = cistern.y0;
			_y1 = cistern.y1;
			_z0 = cistern.z0;
			_z1 = cistern.z1;
			_maxMass = cistern.maxMass;
			_maxVolume = cistern.maxVolume;
			_CalcMass = cistern.CalcMass;
			_CalcVolume = cistern.CalcVolume;
			_percentOfVolume = cistern.percentOfVolume;
			_volumeDensity = cistern.volumeDensity;
			_waterLevel = cistern.waterLevel;
			wtrl = cistern.wtrl;
		}
	}

	public float h
	{
		get
		{
			return _h;
		}
		set
		{
			Cistern cistern = new Cistern(Name, TextDecriptionOfStation, new p3d(x0, y0, z0), new p3d(x1, y1, z0 + value), volumeDensity);
			cistern.percentOfVolume = percentOfVolume;
			Stations = cistern.Stations;
			_TextDecriptionOfStation = cistern.TextDecriptionOfStation;
			_l = cistern.l;
			_b = cistern.b;
			_h = cistern.h;
			Name = cistern.Name;
			_x0 = cistern.x0;
			_x1 = cistern.x1;
			_y0 = cistern.y0;
			_y1 = cistern.y1;
			_z0 = cistern.z0;
			_z1 = cistern.z1;
			_maxMass = cistern.maxMass;
			_maxVolume = cistern.maxVolume;
			_CalcMass = cistern.CalcMass;
			_CalcVolume = cistern.CalcVolume;
			_percentOfVolume = cistern.percentOfVolume;
			_volumeDensity = cistern.volumeDensity;
			_waterLevel = cistern.waterLevel;
			wtrl = cistern.wtrl;
		}
	}

	public float x0
	{
		get
		{
			return _x0;
		}
		set
		{
			Cistern cistern = new Cistern(Name, TextDecriptionOfStation, new p3d(value, y0, z0), new p3d(x1, y1, z1), volumeDensity);
			cistern.percentOfVolume = percentOfVolume;
			Stations = cistern.Stations;
			_TextDecriptionOfStation = cistern.TextDecriptionOfStation;
			_l = cistern.l;
			_b = cistern.b;
			_h = cistern.h;
			Name = cistern.Name;
			_x0 = cistern.x0;
			_x1 = cistern.x1;
			_y0 = cistern.y0;
			_y1 = cistern.y1;
			_z0 = cistern.z0;
			_z1 = cistern.z1;
			_maxMass = cistern.maxMass;
			_maxVolume = cistern.maxVolume;
			_CalcMass = cistern.CalcMass;
			_CalcVolume = cistern.CalcVolume;
			_percentOfVolume = cistern.percentOfVolume;
			_volumeDensity = cistern.volumeDensity;
			_waterLevel = cistern.waterLevel;
			wtrl = cistern.wtrl;
		}
	}

	public float y0
	{
		get
		{
			return _y0;
		}
		set
		{
			Cistern cistern = new Cistern(Name, TextDecriptionOfStation, new p3d(x0, value, z0), new p3d(x1, y1, z1), volumeDensity);
			cistern.percentOfVolume = percentOfVolume;
			Stations = cistern.Stations;
			_TextDecriptionOfStation = cistern.TextDecriptionOfStation;
			_l = cistern.l;
			_b = cistern.b;
			_h = cistern.h;
			Name = cistern.Name;
			_x0 = cistern.x0;
			_x1 = cistern.x1;
			_y0 = cistern.y0;
			_y1 = cistern.y1;
			_z0 = cistern.z0;
			_z1 = cistern.z1;
			_maxMass = cistern.maxMass;
			_maxVolume = cistern.maxVolume;
			_CalcMass = cistern.CalcMass;
			_CalcVolume = cistern.CalcVolume;
			_percentOfVolume = cistern.percentOfVolume;
			_volumeDensity = cistern.volumeDensity;
			_waterLevel = cistern.waterLevel;
			wtrl = cistern.wtrl;
		}
	}

	public float z0
	{
		get
		{
			return _z0;
		}
		set
		{
			Cistern cistern = new Cistern(Name, TextDecriptionOfStation, new p3d(x0, y0, value), new p3d(x1, y1, z1), volumeDensity);
			cistern.percentOfVolume = percentOfVolume;
			Stations = cistern.Stations;
			_TextDecriptionOfStation = cistern.TextDecriptionOfStation;
			_l = cistern.l;
			_b = cistern.b;
			_h = cistern.h;
			Name = cistern.Name;
			_x0 = cistern.x0;
			_x1 = cistern.x1;
			_y0 = cistern.y0;
			_y1 = cistern.y1;
			_z0 = cistern.z0;
			_z1 = cistern.z1;
			_maxMass = cistern.maxMass;
			_maxVolume = cistern.maxVolume;
			_CalcMass = cistern.CalcMass;
			_CalcVolume = cistern.CalcVolume;
			_percentOfVolume = cistern.percentOfVolume;
			_volumeDensity = cistern.volumeDensity;
			_waterLevel = cistern.waterLevel;
			wtrl = cistern.wtrl;
		}
	}

	public float x1
	{
		get
		{
			return _x1;
		}
		set
		{
			Cistern cistern = new Cistern(Name, TextDecriptionOfStation, new p3d(x0, y0, z0), new p3d(value, y1, z1), volumeDensity);
			cistern.percentOfVolume = percentOfVolume;
			Stations = cistern.Stations;
			_TextDecriptionOfStation = cistern.TextDecriptionOfStation;
			_l = cistern.l;
			_b = cistern.b;
			_h = cistern.h;
			Name = cistern.Name;
			_x0 = cistern.x0;
			_x1 = cistern.x1;
			_y0 = cistern.y0;
			_y1 = cistern.y1;
			_z0 = cistern.z0;
			_z1 = cistern.z1;
			_maxMass = cistern.maxMass;
			_maxVolume = cistern.maxVolume;
			_CalcMass = cistern.CalcMass;
			_CalcVolume = cistern.CalcVolume;
			_percentOfVolume = cistern.percentOfVolume;
			_volumeDensity = cistern.volumeDensity;
			_waterLevel = cistern.waterLevel;
			wtrl = cistern.wtrl;
		}
	}

	public float y1
	{
		get
		{
			return _y1;
		}
		set
		{
			Cistern cistern = new Cistern(Name, TextDecriptionOfStation, new p3d(x0, y0, z0), new p3d(x1, value, z1), volumeDensity);
			cistern.percentOfVolume = percentOfVolume;
			Stations = cistern.Stations;
			_TextDecriptionOfStation = cistern.TextDecriptionOfStation;
			_l = cistern.l;
			_b = cistern.b;
			_h = cistern.h;
			Name = cistern.Name;
			_x0 = cistern.x0;
			_x1 = cistern.x1;
			_y0 = cistern.y0;
			_y1 = cistern.y1;
			_z0 = cistern.z0;
			_z1 = cistern.z1;
			_maxMass = cistern.maxMass;
			_maxVolume = cistern.maxVolume;
			_CalcMass = cistern.CalcMass;
			_CalcVolume = cistern.CalcVolume;
			_percentOfVolume = cistern.percentOfVolume;
			_volumeDensity = cistern.volumeDensity;
			_waterLevel = cistern.waterLevel;
			wtrl = cistern.wtrl;
		}
	}

	public float z1
	{
		get
		{
			return _z1;
		}
		set
		{
			Cistern cistern = new Cistern(Name, TextDecriptionOfStation, new p3d(x0, y0, z0), new p3d(x1, y1, value), volumeDensity);
			cistern.percentOfVolume = percentOfVolume;
			Stations = cistern.Stations;
			_TextDecriptionOfStation = cistern.TextDecriptionOfStation;
			_l = cistern.l;
			_b = cistern.b;
			_h = cistern.h;
			Name = cistern.Name;
			_x0 = cistern.x0;
			_x1 = cistern.x1;
			_y0 = cistern.y0;
			_y1 = cistern.y1;
			_z0 = cistern.z0;
			_z1 = cistern.z1;
			_maxMass = cistern.maxMass;
			_maxVolume = cistern.maxVolume;
			_CalcMass = cistern.CalcMass;
			_CalcVolume = cistern.CalcVolume;
			_percentOfVolume = cistern.percentOfVolume;
			_volumeDensity = cistern.volumeDensity;
			_waterLevel = cistern.waterLevel;
			wtrl = cistern.wtrl;
		}
	}

	public float percentOfVolume
	{
		get
		{
			return _percentOfVolume;
		}
		set
		{
			wtrl = new Waterlanding_Volume_Heel_Trim(maxVolume * value / 100f, 0f, 0f, Stations, 0.001f, volumeDensity);
			_maxMass = STEB2.Volume.Tools.GetMaxVolume(Stations) * volumeDensity;
			_maxVolume = STEB2.Volume.Tools.GetMaxVolume(Stations);
			_CalcMass = wtrl.V * volumeDensity;
			_CalcVolume = wtrl.V;
			_percentOfVolume = value;
			_volumeDensity = volumeDensity;
			_waterLevel = wtrl.wlPosition.Z - z0;
		}
	}

	public float volumeDensity
	{
		get
		{
			return _volumeDensity;
		}
		set
		{
			wtrl = new Waterlanding_Volume_Heel_Trim(CalcVolume, 0f, 0f, Stations, 0.001f, value);
			_maxMass = STEB2.Volume.Tools.GetMaxVolume(Stations) * volumeDensity;
			_maxVolume = STEB2.Volume.Tools.GetMaxVolume(Stations);
			_CalcMass = wtrl.V * volumeDensity;
			_CalcVolume = wtrl.V;
			_percentOfVolume = CalcVolume / maxVolume * 100f;
			_volumeDensity = value;
			_waterLevel = wtrl.wlPosition.Z - z0;
		}
	}

	public string TextDecriptionOfStation
	{
		get
		{
			return _TextDecriptionOfStation;
		}
		set
		{
			Cistern cistern = new Cistern(Name, value, new p3d(x0, y0, z0), new p3d(x1, y1, z1), volumeDensity);
			cistern.percentOfVolume = percentOfVolume;
			Stations = cistern.Stations;
			_TextDecriptionOfStation = value;
			_l = cistern.l;
			_b = cistern.b;
			_h = cistern.h;
			Name = cistern.Name;
			_x0 = cistern.x0;
			_x1 = cistern.x1;
			_y0 = cistern.y0;
			_y1 = cistern.y1;
			_z0 = cistern.z0;
			_z1 = cistern.z1;
			_maxMass = cistern.maxMass;
			_maxVolume = cistern.maxVolume;
			_CalcMass = cistern.CalcMass;
			_CalcVolume = cistern.CalcVolume;
			_percentOfVolume = cistern.percentOfVolume;
			_volumeDensity = cistern.volumeDensity;
			_waterLevel = cistern.waterLevel;
			wtrl = cistern.wtrl;
		}
	}

	public Cistern(string Name, string TextDecriptionOfStation, float volumeDensity)
	{
		Stations = STEB2.Volume.Tools.TextToStations(TextDecriptionOfStation);
		Stations = STEB2.Stations.Tools.SortStations(Stations);
		p3d[] boundPointsOfVolume = STEB2.Volume.Tools.GetBoundPointsOfVolume(Stations);
		_TextDecriptionOfStation = TextDecriptionOfStation;
		_l = boundPointsOfVolume[1].X - boundPointsOfVolume[0].X;
		_b = boundPointsOfVolume[1].Y - boundPointsOfVolume[0].Y;
		_h = boundPointsOfVolume[1].Z - boundPointsOfVolume[0].Z;
		this.Name = Name;
		_x0 = boundPointsOfVolume[0].X;
		_x1 = boundPointsOfVolume[1].X;
		_y0 = boundPointsOfVolume[0].Y;
		_y1 = boundPointsOfVolume[1].Y;
		_z0 = boundPointsOfVolume[0].Z;
		_z1 = boundPointsOfVolume[1].Z;
		_maxMass = STEB2.Volume.Tools.GetMaxVolume(Stations) * volumeDensity;
		_maxVolume = STEB2.Volume.Tools.GetMaxVolume(Stations);
		_CalcMass = _maxMass;
		_CalcVolume = _maxVolume;
		_percentOfVolume = 100f;
		_volumeDensity = volumeDensity;
		_waterLevel = _h;
		wtrl = new Waterlanding_Volume_Heel_Trim(_CalcVolume, 0f, 0f, Stations, 0.001f, _volumeDensity);
	}

	public Cistern(string Name, string TextDecriptionOfStation, float l, float b, float h, float volumeDensity)
	{
		Stations = STEB2.Volume.Tools.TextToStations(TextDecriptionOfStation);
		Stations = STEB2.Stations.Tools.SortStations(Stations);
		Stations = STEB2.Volume.Tools.GetAffinnStations(Stations, l, b, h, 0f, 0f, 0f);
		p3d[] boundPointsOfVolume = STEB2.Volume.Tools.GetBoundPointsOfVolume(Stations);
		_TextDecriptionOfStation = TextDecriptionOfStation;
		_l = boundPointsOfVolume[1].X - boundPointsOfVolume[0].X;
		_b = boundPointsOfVolume[1].Y - boundPointsOfVolume[0].Y;
		_h = boundPointsOfVolume[1].Z - boundPointsOfVolume[0].Z;
		this.Name = Name;
		_x0 = boundPointsOfVolume[0].X;
		_x1 = boundPointsOfVolume[1].X;
		_y0 = boundPointsOfVolume[0].Y;
		_y1 = boundPointsOfVolume[1].Y;
		_z0 = boundPointsOfVolume[0].Z;
		_z1 = boundPointsOfVolume[1].Z;
		_maxMass = STEB2.Volume.Tools.GetMaxVolume(Stations) * volumeDensity;
		_maxVolume = STEB2.Volume.Tools.GetMaxVolume(Stations);
		_CalcMass = _maxMass;
		_CalcVolume = _maxVolume;
		_percentOfVolume = 100f;
		_volumeDensity = volumeDensity;
		_waterLevel = _h;
		wtrl = new Waterlanding_Volume_Heel_Trim(_CalcVolume, 0f, 0f, Stations, 0.001f, _volumeDensity);
	}

	public Cistern(string Name, float kx, float ky, float kz, string TextDecriptionOfStation, float volumeDensity)
	{
		Stations = STEB2.Volume.Tools.TextToStations(TextDecriptionOfStation);
		Stations = STEB2.Stations.Tools.SortStations(Stations);
		Stations = STEB2.Volume.Tools.GetAffinnStations(Stations, 0f, 0f, 0f, kx, ky, kz);
		p3d[] boundPointsOfVolume = STEB2.Volume.Tools.GetBoundPointsOfVolume(Stations);
		_TextDecriptionOfStation = TextDecriptionOfStation;
		_l = boundPointsOfVolume[1].X - boundPointsOfVolume[0].X;
		_b = boundPointsOfVolume[1].Y - boundPointsOfVolume[0].Y;
		_h = boundPointsOfVolume[1].Z - boundPointsOfVolume[0].Z;
		this.Name = Name;
		_x0 = boundPointsOfVolume[0].X;
		_x1 = boundPointsOfVolume[1].X;
		_y0 = boundPointsOfVolume[0].Y;
		_y1 = boundPointsOfVolume[1].Y;
		_z0 = boundPointsOfVolume[0].Z;
		_z1 = boundPointsOfVolume[1].Z;
		_maxMass = STEB2.Volume.Tools.GetMaxVolume(Stations) * volumeDensity;
		_maxVolume = STEB2.Volume.Tools.GetMaxVolume(Stations);
		_CalcMass = _maxMass;
		_CalcVolume = _maxVolume;
		_percentOfVolume = 100f;
		_volumeDensity = volumeDensity;
		_waterLevel = _h;
		wtrl = new Waterlanding_Volume_Heel_Trim(_CalcVolume, 0f, 0f, Stations, 0.001f, _volumeDensity);
	}

	public Cistern(string Name, string TextDecriptionOfStation, p3d startPoint, p3d endPoint, float volumeDensity)
	{
		Stations = STEB2.Volume.Tools.TextToStations(TextDecriptionOfStation);
		Stations = STEB2.Stations.Tools.SortStations(Stations);
		Stations = STEB2.Volume.Tools.GetAffinnStations(Stations, endPoint.X - startPoint.X, endPoint.Y - startPoint.Y, endPoint.Z - startPoint.Z, 0f, 0f, 0f);
		STEB2.Volume.Tools.GetGabariteOfVolume(Stations);
		p3d[] boundPointsOfVolume = STEB2.Volume.Tools.GetBoundPointsOfVolume(Stations);
		float dx = startPoint.X - boundPointsOfVolume[0].X;
		float dy = startPoint.Y - boundPointsOfVolume[0].Y;
		float dz = startPoint.Z - boundPointsOfVolume[0].Z;
		Stations = STEB2.Volume.Tools.MoveVolume(Stations, dx, dy, dz);
		_TextDecriptionOfStation = TextDecriptionOfStation;
		_l = boundPointsOfVolume[1].X - boundPointsOfVolume[0].X;
		_b = boundPointsOfVolume[1].Y - boundPointsOfVolume[0].Y;
		_h = boundPointsOfVolume[1].Z - boundPointsOfVolume[0].Z;
		this.Name = Name;
		_x0 = startPoint.X;
		_x1 = endPoint.X;
		_y0 = startPoint.Y;
		_y1 = endPoint.Y;
		_z0 = startPoint.Z;
		_z1 = endPoint.Z;
		_maxMass = STEB2.Volume.Tools.GetMaxVolume(Stations) * volumeDensity;
		_maxVolume = STEB2.Volume.Tools.GetMaxVolume(Stations);
		_CalcMass = _maxMass;
		_CalcVolume = _maxVolume;
		_percentOfVolume = 100f;
		_volumeDensity = volumeDensity;
		_waterLevel = _h;
		wtrl = new Waterlanding_Volume_Heel_Trim(_CalcVolume, 0f, 0f, Stations, 0.001f, _volumeDensity);
	}
}
