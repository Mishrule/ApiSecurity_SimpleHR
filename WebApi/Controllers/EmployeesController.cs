using Application.Features.Employees.Commands;
using Application.Features.Employees.Queries;
using Common.Requests.Employees;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class EmployeesController : AppBaseController<EmployeesController>
    {
        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeRequest employee)
        {
            var response = await MediatorSender.Send(new CreateEmployeeCommand { CreateEmployeeRequest = employee });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateEmployee([FromBody] UpdateEmployeeRequest employee)
        {
            var response = await MediatorSender.Send(new UpdateEmployeeCommand { UpdateEmployeeRequest = employee });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpDelete("{employeeId}")]
        public async Task<IActionResult> DeleteEmployee(int employeeId)
        {
            var response = await MediatorSender.Send(new DeleteEmployeeCommand { EmployeeId = employeeId });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeeList()
        {
            var response = await MediatorSender.Send(new GetEmployeesQuery());
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpGet("{employeeId}")]
        public async Task<IActionResult> GetEmployeeById(int employeeId)
        {
            var response = await MediatorSender.Send(new GetEmployeeByIdQuery(employeeId));
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }
    }
}
