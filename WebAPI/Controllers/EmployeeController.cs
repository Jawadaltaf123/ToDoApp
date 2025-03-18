using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models.Common;
using WebAPI.Models.ModelClasses;
using WebAPI.Service;

namespace WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        [HttpGet("GetUsers")]
        public async Task<Response> GetUsers()
        {
            return await employeeService.GetUsers();
        }

        [HttpPost("AddUser")]
        public async Task<Response> AddUser(SignupRequest user)
        {
            return await employeeService.AddUser(user);
        }

        [HttpPost("AuthenticateUser")]
        public async Task<Response> AuthenticateUser(LoginRequest user)
        {
            return await employeeService.AuthenticateUser(user);
        }

        [HttpPost("AddTask")]
        public async Task<Response> AddTask(TaskDto task)
        {
            return await employeeService.AddTask(task);
        }

        [HttpGet("GetAllTasks")]
        public async Task<Response> GetAllTasks()
        {
            return await employeeService.GetAllTasks();
        }

        [HttpPut("UpdatedTask/{id}")]
        public async Task<Response> UpdatedTask(long id, TaskDto updatedTask)
        {
            return await employeeService.UpdatedTask(id, updatedTask);
        }

        [HttpGet("FilterTaskById/{id}")]
        public async Task<Response> FilterTaskById(long id)
        {
            return await employeeService.FilterTaskById(id);
        }

        [HttpDelete("DeleteTaskById/{id}")]
        public async Task<Response> DeleteTaskById(long id)
        {
            return await employeeService.DeleteTaskById(id);
        }


    }
}
