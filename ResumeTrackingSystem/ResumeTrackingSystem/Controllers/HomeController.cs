using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResumeTrackingSystem.Data;
using ResumeTrackingSystem.Model;
using ResumeTrackingSystemAPI.Model;

[Route("api/[controller]")]
[ApiController]
public class HomeController : ControllerBase
{
    private readonly IEmployeeDataAccess _employeeDataAccess;
    private readonly IConverter _converter;
    private readonly EmployeeDbContext _employeeDb;

    public HomeController(IEmployeeDataAccess employeeDataAccess, IConverter converter, EmployeeDbContext employeeDb)
    {
        _employeeDataAccess = employeeDataAccess;
        _converter = converter;
        _employeeDb = employeeDb;
    }

    [HttpPost("create")]
    [Authorize]
    public IActionResult CreateEmployee([FromForm] Employee employeeDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine($"ModelState Error: {error.ErrorMessage}");
            }
            return BadRequest(ModelState);
        }

        var employee = new Employee
        {
            employeeid=employeeDto.employeeid,
            firstname = employeeDto.firstname,
            lastname = employeeDto.lastname,
            email = employeeDto.email,
            phonenumber = employeeDto.phonenumber,
            address = employeeDto.address,
            city = employeeDto.city,
            country = employeeDto.country,
            yearsofexperience = employeeDto.yearsofexperience,
            dateofbirth = employeeDto.dateofbirth,
            skills = employeeDto.skills
        };

        if (employeeDto.profilepicturefile != null)
        {
            using (var memoryStream = new MemoryStream())
            {
                employeeDto.profilepicturefile.CopyTo(memoryStream);
                employee.profilepicture = memoryStream.ToArray();
            }
        }

        _employeeDataAccess.AddEmployee(employee);
        return CreatedAtAction(nameof(GetEmployee), new { id = employee.employeeid }, employee);
    }

    [HttpGet("getById/{id}")]
    [Authorize]
    public IActionResult GetEmployee(int id)
    {
        var employee = _employeeDataAccess.GetEmployee(id);
        if (employee == null)
        {
            return NotFound();
        }
        return Ok(employee);
    }

    [HttpGet("getAllEmps")]
    [Authorize]
    public IActionResult GetAllEmps()
    {
        return Ok(_employeeDataAccess.GetAllEmployees().ToList<Employee>());
    }

    [HttpPut("update/{id}")]
    [Authorize]
    public IActionResult UpdateEmployee(int id, [FromForm] Employee employeeDto)
    {
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine($"Validation Error: {error.ErrorMessage}");
            }
            return BadRequest(ModelState);
        }

        var employee = _employeeDataAccess.GetEmployee(id);
        if (employee == null)
        {
            return NotFound();
        }

        employee.firstname = employeeDto.firstname;
        employee.lastname = employeeDto.lastname;
        employee.email = employeeDto.email;
        employee.phonenumber = employeeDto.phonenumber;
        employee.address = employeeDto.address;
        employee.city = employeeDto.city;
        employee.country = employeeDto.country;
        employee.yearsofexperience = employeeDto.yearsofexperience;
        employee.dateofbirth = employeeDto.dateofbirth;
        employee.skills = employeeDto.skills;

        if (employeeDto.profilepicturefile != null)
        {
            using (var memoryStream = new MemoryStream())
            {
                employeeDto.profilepicturefile.CopyTo(memoryStream);
                employee.profilepicture = memoryStream.ToArray();
            }
        }

        _employeeDataAccess.UpdateEmployee(employee);
        return NoContent();
    }

    [HttpDelete("delete/{id}")]
    [Authorize]
    public IActionResult DeleteEmployee(int id)
    {
        var employee = _employeeDataAccess.GetEmployee(id);
        if (employee == null)
        {
            return NotFound();
        }

        _employeeDataAccess.DeleteEmployee(id);
        return Ok(new { msg = "Record Deleted" });
    }

    [HttpGet("search")]
    public IActionResult SearchEmployees(string? name, string? email, string? phone, string? skills, int? experience)
    {
        var employees = _employeeDataAccess.SearchEmployees(name, email, phone, skills, experience);
        return Ok(employees);
    }

    [HttpGet("{id}/download-profile-picture")]
    public IActionResult DownloadProfile(int id)
    {
        var resume = _employeeDb.Employees.FirstOrDefault(d => d.employeeid == id);
        if (resume != null)
        {
            return File(resume.profilepicture, "application/octet-stream", resume.firstname + ".jpg");
        }
        else
        {
            return NotFound("File not Found");
        }
    }

    [HttpGet("download-pdf")]
    public IActionResult DownloadPdf(int id)
    {
        var data = _employeeDb.Employees.Find(id);

        if (data != null)
        {
            var emp = new Employee
            {
                firstname = data.firstname,
                lastname = data.lastname,
                email = data.email,
                phonenumber = data.phonenumber,
                address = data.address,
                city = data.city,
                country = data.country,
                yearsofexperience = data.yearsofexperience,
                dateofbirth = data.dateofbirth,
                skills = data.skills,
                profilepicture=data.profilepicture
            };
            var pdfBytes = _employeeDataAccess.GeneratePdf(emp);
            return File(pdfBytes, "application/pdf", "person_info.pdf");
        }
        else
        {
            return NotFound("Data not Found");
        }
    }

    [HttpGet("image/{id}")]
    public async Task<IActionResult> GetImage(int id)
    {
        var employee = await _employeeDb.Employees.FindAsync(id);
        if (employee == null || employee.profilepicture == null)
            return NotFound();

        return File(employee.profilepicture, "image/jpg");
    }
}
