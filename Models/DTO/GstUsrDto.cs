using System;
using Configuration;

namespace Models.DTO;

public class GstUsrInfoDbDto
{
    public string Title {get;  set;}
    public int NrSeededAttractions { get; set; } = 0;
    public int NrUnseededAttractions { get; set; } = 0;

    public int NrSeededComments { get; set; } = 0;
    public int NrUnseededComments { get; set; } = 0;

    public int NrSeededAdresses { get; set; } = 0;
    public int NrUnseededAdresses { get; set; } = 0;

    public int NrSeededCategories { get; set; } = 0;
    public int NrUnseededCategories { get; set; } = 0;
}

public class GstUsrInfoZoosDto
{
    public string Country { get; set; } = null;
    public string City { get; set; } = null;
    public int NrZoos { get; set; } = 0;
}

public class GstUsrInfoAnimalsDto
{
    public string Country { get; set; } = null;
    public string City { get; set; } = null;
    public string ZooName { get; set; } = null;
    public int NrAnimals { get; set; } = 0;
}

public class GstUsrInfoEmployeesDto
{
    public string Country { get; set; } = null;
    public string City { get; set; } = null;
    public string ZooName { get; set; } = null;
    public int NrEmployees { get; set; } = 0;
}

public class GstUsrInfoAllDto
{
    public GstUsrInfoDbDto Db { get; set; } = null;
    public List<GstUsrInfoZoosDto> Zoos { get; set; } = null;
    public List<GstUsrInfoAnimalsDto> Animals { get; set; } = null;
    public List<GstUsrInfoEmployeesDto> Employees { get; set; } = null;
}


