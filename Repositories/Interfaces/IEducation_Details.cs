using System.Net.NetworkInformation;

public interface IEducation_Details
{
    Task<int> AddEducationDetails(t_Education_Details t_Education);
    Task<List<t_Education_Details>> GetEducation_Details(int id);

    Task<int> UpdateEducationDetails(t_Education_Details t_Education);

    Task<int> DeleteEducationDetails(int id);

    Task<t_Education_Details> GetOneEducation(int id);
}