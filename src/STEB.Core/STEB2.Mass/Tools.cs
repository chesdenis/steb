using System.Collections.Generic;
using STEB2.Cisterns;
using STEB2.Compartments;
using STEB2.Containers;
using STEB2.Geometry;
using STEB2.Volume;

namespace STEB2.Mass;

public class Tools
{
	public static void AddItem(MassItem itm, ref List<MassTable> lstTableOfMass)
	{
		foreach (MassTable item in lstTableOfMass)
		{
			foreach (MassCategory lstMassCategory in item.lstMassCategories)
			{
				foreach (MassItem lstMassItem in lstMassCategory.lstMassItems)
				{
					if (lstMassItem.id == itm.id)
					{
						return;
					}
				}
			}
		}
		foreach (MassTable item2 in lstTableOfMass)
		{
			foreach (MassCategory lstMassCategory2 in item2.lstMassCategories)
			{
				if (itm.MassTableName == item2.Name && itm.CatName == lstMassCategory2.Name)
				{
					lstMassCategory2.lstMassItems.Add(itm);
					return;
				}
			}
		}
		foreach (MassTable item3 in lstTableOfMass)
		{
			if (itm.MassTableName == item3.Name)
			{
				MassCategory massCategory = new MassCategory();
				massCategory.Name = itm.CatName;
				massCategory.lstMassItems.Add(itm);
				item3.lstMassCategories.Add(massCategory);
				return;
			}
		}
		MassTable massTable = new MassTable();
		MassCategory massCategory2 = new MassCategory();
		massTable.Name = itm.MassTableName;
		massCategory2.Name = itm.CatName;
		massCategory2.lstMassItems.Add(itm);
		massTable.lstMassCategories.Add(massCategory2);
		lstTableOfMass.Add(massTable);
	}

	public static void DeleteItem(string id, ref List<MassTable> lstTableOfMass)
	{
		foreach (MassTable item in lstTableOfMass)
		{
			foreach (MassCategory lstMassCategory in item.lstMassCategories)
			{
				foreach (MassItem lstMassItem in lstMassCategory.lstMassItems)
				{
					if (lstMassItem.id == id)
					{
						lstMassCategory.lstMassItems.Remove(lstMassItem);
						try
						{
							ClearTrash(ref lstTableOfMass);
							return;
						}
						catch
						{
							return;
						}
					}
				}
			}
		}
	}

	public static void ModifyItem(string oldId, MassItem itm, ref List<MassTable> lstTableOfMass)
	{
		foreach (MassTable item in lstTableOfMass)
		{
			foreach (MassCategory lstMassCategory in item.lstMassCategories)
			{
				foreach (MassItem lstMassItem in lstMassCategory.lstMassItems)
				{
					if (lstMassItem.id == oldId)
					{
						lstMassItem.Area = itm.Area;
						lstMassItem.CatName = itm.CatName;
						lstMassItem.Density = itm.Density;
						lstMassItem.GPosition = itm.GPosition;
						lstMassItem.id = itm.id;
						lstMassItem.Mass = itm.Mass;
						lstMassItem.MassTableName = itm.MassTableName;
						lstMassItem.Mp = itm.Mp;
						lstMassItem.Name = itm.Name;
						lstMassItem.Thickness = itm.Thickness;
						goto end_IL_010a;
					}
				}
			}
			continue;
			end_IL_010a:
			break;
		}
		List<MassItem> list = new List<MassItem>();
		foreach (MassTable item2 in lstTableOfMass)
		{
			foreach (MassCategory lstMassCategory2 in item2.lstMassCategories)
			{
				foreach (MassItem lstMassItem2 in lstMassCategory2.lstMassItems)
				{
					list.Add(lstMassItem2);
				}
			}
		}
		lstTableOfMass.Clear();
		try
		{
			foreach (MassItem item3 in list)
			{
				item3.id = item3.MassTableName + "." + item3.CatName + "." + item3.Name;
				bool flag = false;
				foreach (MassTable item4 in lstTableOfMass)
				{
					if (item4.Name == item3.MassTableName)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					MassTable massTable = new MassTable();
					massTable.Name = item3.MassTableName;
					lstTableOfMass.Add(massTable);
				}
			}
			foreach (MassItem item5 in list)
			{
				foreach (MassTable item6 in lstTableOfMass)
				{
					if (!(item6.Name == item5.MassTableName))
					{
						continue;
					}
					bool flag2 = false;
					foreach (MassCategory lstMassCategory3 in item6.lstMassCategories)
					{
						if (lstMassCategory3.Name == item5.CatName)
						{
							flag2 = true;
						}
					}
					if (!flag2)
					{
						MassCategory massCategory = new MassCategory();
						massCategory.Name = item5.CatName;
						item6.lstMassCategories.Add(massCategory);
					}
				}
			}
			foreach (MassItem item7 in list)
			{
				foreach (MassTable item8 in lstTableOfMass)
				{
					foreach (MassCategory lstMassCategory4 in item8.lstMassCategories)
					{
						if (item8.Name == item7.MassTableName && lstMassCategory4.Name == item7.CatName)
						{
							lstMassCategory4.lstMassItems.Add(item7);
						}
					}
				}
			}
		}
		catch
		{
		}
		try
		{
			ClearTrash(ref lstTableOfMass);
		}
		catch
		{
		}
	}

	public static void ClearTrash(ref List<MassTable> lstTableOfMass)
	{
		for (int i = 0; i < lstTableOfMass.Count; i++)
		{
			if (lstTableOfMass[i].lstMassCategories.Count == 0)
			{
				lstTableOfMass.RemoveAt(i);
				i = 0;
				continue;
			}
			for (int j = 0; j < lstTableOfMass[i].lstMassCategories.Count; j++)
			{
				if (lstTableOfMass[i].lstMassCategories[j].lstMassItems.Count == 0)
				{
					lstTableOfMass[i].lstMassCategories.RemoveAt(j);
					j = 0;
					i = 0;
					break;
				}
			}
		}
	}

	public static void CalcG(List<MassTable> lstMassTables)
	{
		foreach (MassTable lstMassTable in lstMassTables)
		{
			lstMassTable.Mass = 0f;
			lstMassTable.Mp.X = 0f;
			lstMassTable.Mp.Y = 0f;
			lstMassTable.Mp.Z = 0f;
			foreach (MassCategory lstMassCategory in lstMassTable.lstMassCategories)
			{
				lstMassCategory.Mass = 0f;
				lstMassCategory.Mp.X = 0f;
				lstMassCategory.Mp.Y = 0f;
				lstMassCategory.Mp.Z = 0f;
				foreach (MassItem lstMassItem in lstMassCategory.lstMassItems)
				{
					lstMassItem.Mp.X = 0f;
					lstMassItem.Mp.Y = 0f;
					lstMassItem.Mp.Z = 0f;
				}
			}
		}
		for (int i = 0; i < lstMassTables.Count; i++)
		{
			for (int j = 0; j < lstMassTables[i].lstMassCategories.Count; j++)
			{
				for (int k = 0; k < lstMassTables[i].lstMassCategories[j].lstMassItems.Count; k++)
				{
					lstMassTables[i].lstMassCategories[j].Mass += lstMassTables[i].lstMassCategories[j].lstMassItems[k].Mass;
					lstMassTables[i].Mass += lstMassTables[i].lstMassCategories[j].lstMassItems[k].Mass;
					float num = lstMassTables[i].lstMassCategories[j].lstMassItems[k].Mass * lstMassTables[i].lstMassCategories[j].lstMassItems[k].GPosition.X;
					float num2 = lstMassTables[i].lstMassCategories[j].lstMassItems[k].Mass * lstMassTables[i].lstMassCategories[j].lstMassItems[k].GPosition.Y;
					float num3 = lstMassTables[i].lstMassCategories[j].lstMassItems[k].Mass * lstMassTables[i].lstMassCategories[j].lstMassItems[k].GPosition.Z;
					lstMassTables[i].lstMassCategories[j].lstMassItems[k].Mp.X = num;
					lstMassTables[i].lstMassCategories[j].lstMassItems[k].Mp.Y = num2;
					lstMassTables[i].lstMassCategories[j].lstMassItems[k].Mp.Z = num3;
					lstMassTables[i].lstMassCategories[j].Mp.X += num;
					lstMassTables[i].lstMassCategories[j].Mp.Y += num2;
					lstMassTables[i].lstMassCategories[j].Mp.Z += num3;
					lstMassTables[i].Mp.X += num;
					lstMassTables[i].Mp.Y += num2;
					lstMassTables[i].Mp.Z += num3;
				}
			}
		}
		for (int l = 0; l < lstMassTables.Count; l++)
		{
			for (int m = 0; m < lstMassTables[l].lstMassCategories.Count; m++)
			{
				try
				{
					lstMassTables[l].lstMassCategories[m].GPosition = new p3d(lstMassTables[l].lstMassCategories[m].Mp.X / lstMassTables[l].lstMassCategories[m].Mass, lstMassTables[l].lstMassCategories[m].Mp.Y / lstMassTables[l].lstMassCategories[m].Mass, lstMassTables[l].lstMassCategories[m].Mp.Z / lstMassTables[l].lstMassCategories[m].Mass);
				}
				catch
				{
				}
			}
			try
			{
				lstMassTables[l].GPosition = new p3d(lstMassTables[l].Mp.X / lstMassTables[l].Mass, lstMassTables[l].Mp.Y / lstMassTables[l].Mass, lstMassTables[l].Mp.Z / lstMassTables[l].Mass);
			}
			catch
			{
			}
		}
	}

	public static List<MassTable> CalcMassGposByCompartmentsContainersBaseMassCisterns(float baseMass, p3d baseGPos, List<Compartment> compartments, List<Container> containers, List<Cistern> cisterns, float heelAngle, float trimAngle, float Precisy)
	{
		List<MassTable> lstTableOfMass = new List<MassTable>();
		MassTable massTable = new MassTable();
		MassCategory massCategory = new MassCategory();
		massTable.Name = "base";
		massCategory.Name = "base";
		massTable.lstMassCategories.Add(massCategory);
		lstTableOfMass.Add(massTable);
		AddItem(new MassItem("base", "Base_Mass_Gpos", "base", baseGPos, baseMass, 0f, 0f, 0f), ref lstTableOfMass);
		foreach (Container container in containers)
		{
			try
			{
				AddItem(new MassItem("base", "container_" + container.Name, "base", new p3d(container.pos.X + container.l / 2f, container.pos.Y, container.pos.Z + container.h / 2f), container.Mass, 0f, 0f, 0f), ref lstTableOfMass);
			}
			catch
			{
			}
		}
		foreach (Compartment compartment in compartments)
		{
			if (compartment.percentOfVolume > 0f)
			{
				try
				{
					float maxVolume = STEB2.Volume.Tools.GetMaxVolume(compartment.CompartmentStations);
					Waterlanding_Volume_Heel_Trim waterlanding_Volume_Heel_Trim = new Waterlanding_Volume_Heel_Trim(maxVolume * compartment.percentOfVolume / 100f, heelAngle, trimAngle, compartment.CompartmentStations, Precisy, compartment.volumeDensity);
					AddItem(new MassItem("base", "compartment_" + compartment.Name, "base", new p3d(waterlanding_Volume_Heel_Trim.CPos.X, waterlanding_Volume_Heel_Trim.CPos.Y, waterlanding_Volume_Heel_Trim.CPos.Z), waterlanding_Volume_Heel_Trim.DSPL * compartment.coefficientPronicaem, 0f, 0f, 0f), ref lstTableOfMass);
					compartment.wtrl = waterlanding_Volume_Heel_Trim;
					compartment.wtrl.V *= compartment.coefficientPronicaem;
					compartment.wtrl.DSPL *= compartment.coefficientPronicaem;
				}
				catch
				{
				}
			}
		}
		foreach (Cistern cistern in cisterns)
		{
			if (cistern.percentOfVolume > 0f)
			{
				try
				{
					float maxVolume2 = STEB2.Volume.Tools.GetMaxVolume(cistern.Stations);
					Waterlanding_Volume_Heel_Trim waterlanding_Volume_Heel_Trim2 = new Waterlanding_Volume_Heel_Trim(maxVolume2 * cistern.percentOfVolume / 100f, heelAngle, trimAngle, cistern.Stations, Precisy, cistern.volumeDensity);
					AddItem(new MassItem("base", "cistern_" + cistern.Name, "base", new p3d(waterlanding_Volume_Heel_Trim2.CPos.X, waterlanding_Volume_Heel_Trim2.CPos.Y, waterlanding_Volume_Heel_Trim2.CPos.Z), waterlanding_Volume_Heel_Trim2.DSPL, 0f, 0f, 0f), ref lstTableOfMass);
					cistern.wtrl = waterlanding_Volume_Heel_Trim2;
				}
				catch
				{
				}
			}
		}
		CalcG(lstTableOfMass);
		return lstTableOfMass;
	}

	public static List<MassTable> CalcMassGpos(float baseMass, p3d baseGPos, float Precisy)
	{
		List<MassTable> lstTableOfMass = new List<MassTable>();
		MassTable massTable = new MassTable();
		MassCategory massCategory = new MassCategory();
		massTable.Name = "base";
		massCategory.Name = "base";
		massTable.lstMassCategories.Add(massCategory);
		lstTableOfMass.Add(massTable);
		AddItem(new MassItem("base", "Base_Mass_Gpos", "base", baseGPos, baseMass, 0f, 0f, 0f), ref lstTableOfMass);
		CalcG(lstTableOfMass);
		return lstTableOfMass;
	}
}
