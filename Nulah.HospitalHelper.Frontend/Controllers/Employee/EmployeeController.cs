using Microsoft.AspNetCore.Mvc;
using Nulah.HospitalHelper.Api.Controllers;
using Nulah.HospitalHelper.Frontend.Models;
using Nulah.HospitalHelper.Frontend.Models.Employee;

namespace Nulah.HospitalHelper.Frontend.Controllers.Employee
{
    [Route("Employee")]
    [LazyUserAuthorise]
    public class EmployeeController : Controller
    {
        private readonly EmployeeApiController _employeeApi;
        private readonly UserApiController _userApi;
        public EmployeeController(EmployeeApiController employeApiController, UserApiController userApiController)
        {
            _employeeApi = employeApiController;
            _userApi = userApiController;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var viewModel = new EmployeeIndexViewModel
            {
                Employees = _employeeApi.GetAllEmployees()
            };


            return View(viewModel);
        }

        [HttpGet]
        [Route("New")]
        public IActionResult New()
        {
            ViewData["Errors"] = TempData["Errors"];

            return View();
        }

        [HttpPost]
        [Route("New")]
        public IActionResult CreateNewEmployee([FromForm] NewEmployeeFormData formData)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(formData.FullName) == true)
            {
                errors.Add("Full name cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(formData.DisplayFirstName) == true)
            {
                errors.Add("Display first name cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(formData.Password) == true)
            {
                errors.Add("Password cannot be empty");
            }

            if (errors.Count > 0)
            {
                TempData["Errors"] = errors;
                return RedirectToAction(nameof(EmployeeController.New));
            }

            var newEmployee = _employeeApi.CreateNewEmployee(formData.FullName, formData.DisplayFirstName, formData.DisplayLastName);

            if (newEmployee == null)
            {
                errors.Add("Failed to create employee");
                TempData["Errors"] = errors;
                return RedirectToAction(nameof(EmployeeController.New));
            }

            var createUser = _userApi.CreateNewUser(newEmployee.EmployeeId, formData.Password);

            if (createUser == false)
            {
                errors.Add("Failed to create user account for employee");
                TempData["Errors"] = errors;
                return RedirectToAction(nameof(EmployeeController.New));
            }

            return RedirectToAction(nameof(EmployeeController.Index));
        }
    }
}
