using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using ResumeTrackingSystem.Model;
using ResumeTrackingSystemAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ResumeTrackingSystem.Data
{
    public class EmployeeDataAccess : IEmployeeDataAccess
    {
        private readonly EmployeeDbContext _employeeDb;

        public EmployeeDataAccess(EmployeeDbContext employeeDb)
        {
            _employeeDb = employeeDb;
        }

        public void AddEmployee(Employee emp)
        {
            _employeeDb.Employees.Add(emp);
            _employeeDb.SaveChanges();
        }

        public void DeleteEmployee(int empid)
        {
            var result = _employeeDb.Employees.Find(empid);
            if (result != null)
            {
                _employeeDb.Employees.Remove(result);
                _employeeDb.SaveChanges();
            }
        }

        public byte[] GeneratePdf(Employee emp)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var writer = new PdfWriter(memoryStream))
                {
                    using (var pdf = new PdfDocument(writer))
                    {
                        Document document = new Document(pdf);

                        document.Add(new Paragraph("Person Information")
                                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                                        .SetFontSize(18));

                        if (emp.profilepicture != null && emp.profilepicture.Length > 0)
                        {
                            var imageData = iText.IO.Image.ImageDataFactory.Create(emp.profilepicture);
                            var image = new Image(imageData);

                            image.SetWidth(100f).SetHeight(100f);
                            document.Add(image);
                        }
                        else
                        {
                            document.Add(new Paragraph("No profile picture available"));
                        }

                        document.Add(new Paragraph($"Firstname: {emp.firstname}"));
                        document.Add(new Paragraph($"Lastname: {emp.lastname}"));
                        document.Add(new Paragraph($"Email: {emp.email}"));
                        document.Add(new Paragraph($"Phonenumber: {emp.phonenumber}"));
                        document.Add(new Paragraph($"Address: {emp.address}"));
                        document.Add(new Paragraph($"City: {emp.city}"));
                        document.Add(new Paragraph($"Country: {emp.country}"));
                        document.Add(new Paragraph($"Skills: {emp.skills}"));
                        document.Add(new Paragraph($"Years Of Experience: {emp.yearsofexperience}"));
                        document.Add(new Paragraph($"Date Of Birth: {emp.dateofbirth}"));
                    }
                }

                return memoryStream.ToArray();
            }
        }

        public List<Employee> GetAllEmployees()
        {
            return _employeeDb.Employees.ToList();
        }

        public Employee GetEmployee(int empid)
        {
            var result = _employeeDb.Employees.Find(empid);
            if (result != null)
            {
                return result;
            }
            else
            {
                throw new Exception("Record not found");
            }
        }

        public List<Employee> SearchEmployees(string? name, string? email, string? phone, string? skills, int? experience)
        {
            var query = _employeeDb.Employees.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(e => e.firstname.Contains(name) || e.lastname.Contains(name));
            }
            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(e => e.email.Contains(email));
            }
            if (!string.IsNullOrEmpty(phone))
            {
                query = query.Where(e => e.phonenumber.Contains(phone));
            }
            if (!string.IsNullOrEmpty(skills))
            {
                query = query.Where(e => e.skills.Contains(skills));
            }
            if (experience.HasValue)
            {
                query = query.Where(e => e.yearsofexperience == experience.Value);
            }

            return query.ToList();
        }

        public void UpdateEmployee(Employee emp)
        {
            _employeeDb.Employees.Update(emp);
            _employeeDb.SaveChanges();
        }
    }
}
