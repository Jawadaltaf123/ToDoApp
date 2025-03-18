using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPI.Models.Common;
using WebAPI.Models.Data;
using WebAPI.Models.ModelClasses;

namespace WebAPI.Service
{
    public interface IEmployeeService
    {
        public Task<Response> GetUsers();
        public Task<Response> AddUser(SignupRequest user);
        public Task<Response> AuthenticateUser(LoginRequest user);

        // Jwt token generate method
        //public Task<Response> GenrateJwtToken(User user);

        // All Todo methods
        public Task<Response> AddTask(TaskDto task);

        public Task<Response> GetAllTasks();
        public Task<Response> UpdatedTask(long id, TaskDto updatedTask);

        public Task<Response> FilterTaskById(long id);
        public Task<Response> DeleteTaskById(long id);
    }


    public class EmployeeService: IEmployeeService
    {
        private readonly EmployeeDbContext dbContext;

        public EmployeeService(EmployeeDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        //Add new User 
        async Task<Response> IEmployeeService.AddUser(SignupRequest user)
        {
            Response response = new Response();
            try
            {

                var newUser = new User
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Password = user.Password,
                    ConfirmPassword = user.ConfirmPassword,

                };

                
                if(newUser.Id  == 0)
                {
                    //check Email exist
                    //if (await checkEmailExist(newUser.Email))
                    //{
                    //    response.ResponseMessage = "Email already exist!";
                    //}

                    //make a hash password
                    var passwordHasher = new PasswordHasher<string>();
                    newUser.Password = passwordHasher.HashPassword(null, newUser.Password);
                    newUser.ConfirmPassword = passwordHasher.HashPassword(null, newUser.ConfirmPassword);
                    
                    // Hash the password
                    dbContext.Users.Add(newUser);
                }
                dbContext.SaveChanges();
                response.StatusCode = 200;
                response.ResponseMessage = "User successfully added!";
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.ResponseMessage = ex.Message;
            }
            return response;
        }


        //Get User List
        async Task<Response> IEmployeeService.GetUsers()
        {
            Response response = new Response();
            try
            {
                List<User> users = dbContext.Users.ToList();
                response.StatusCode = 200;
                response.ResponseMessage = "Okay";
                response.ResultData = users;
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.ResponseMessage = ex.Message;
            }
            return response;
        }

        //Login authenticate work
        //async Task<Response> IEmployeeService.AuthenticateUser(LoginRequest user)
        //{
        //    Response response = new Response();
        //    try
        //    {
        //        if(user == null)
        //        {
        //            response.StatusCode = 400;
        //            response.ResponseMessage = "Not Found!";
        //            return response;
        //        }
        //        var passwordHasher = new PasswordHasher<string>();

        //        var userData = dbContext.Users.FirstOrDefault(x => x.Email == user.Email);
        //        //var Userdata = dbContext.Users.ToList().Where
        //        //    (x => x.Email == user.Email).FirstOrDefault();

        //        //decode password from passwordHasher
        //        var isPasswordValid = PasswordVerificationResult.Failed; 
        //        if (userData != null)
        //        {
        //            isPasswordValid = passwordHasher.VerifyHashedPassword(null, userData.Password, user.Password);
        //        }

        //        if (isPasswordValid == PasswordVerificationResult.Success)
        //        {
        //            response.StatusCode = 200;
        //            response.ResponseMessage = "User Authenticated";
        //            response.ResultData = userData;
        //        }
        //        else
        //        {
        //            response.StatusCode = 404;
        //            response.ResponseMessage = "User Not Found!";
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        response.StatusCode = 500;
        //        response.ResponseMessage = ex.Message;
        //    }
        //    return response;


        //}

        //Login with Jwt 
        public async Task<Response> AuthenticateUser(LoginRequest loginRequest)
        {
            Response response = new Response();
            try
            {
                if(loginRequest == null)
                {
                    response.StatusCode = 400;
                    response.ResultData = "Not Found User!";
                    return response;
                }
                var passwordHasher = new PasswordHasher<string>();

                var userData = dbContext.Users.FirstOrDefault(u => u.Email == loginRequest.Email);

                // Verify password using PasswordHasher
                var passHasher = new PasswordHasher<string>();
                var isPasswordValid = passHasher.VerifyHashedPassword(null, userData.Password, loginRequest.Password);
                

                if(isPasswordValid != PasswordVerificationResult.Success)
                {
                    response.StatusCode = 401;
                    response.ResponseMessage = "Invalid Password!";
                    return response;
                }

                // Generate JWT token
                //var tokenResponce = await GenrateJwtToken(userData);

                //if(tokenResponce.StatusCode == 200)
                //{
                response.StatusCode = 200;
                response.ResponseMessage = "User Authenticate!";
                response.ResultData = userData;
                ////response.ResultData = tokenResponce.ResultData;

                //}   
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.ResponseMessage = ex.Message;
            }
            return response;
        }




        //Add Todo task
        public async Task<Response> AddTask(TaskDto task)
        {
            Response response = new Response();
            try
            {
                // Check if the user exists
                var user = dbContext.Users.FirstOrDefault(u => u.Id == task.UserId);
                if (user == null)
                {
                    response.StatusCode = 404;
                    response.ResponseMessage = "User not found";
                    return response;
                }

                // Create a new task
                var newTask = new TaskDto
                {
                    TaskTitle = task.TaskTitle,
                    IsImportant = task.IsImportant,
                    IsCompleted = task.IsCompleted,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = user.UserName,
                    UserId = task.UserId // Link task to user

                };

               
                dbContext.Tasks.Add(newTask);
                await dbContext.SaveChangesAsync(); 

                response.StatusCode = 200;
                response.ResponseMessage = "Task added successfully";
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.ResponseMessage = ex.Message;
            }
            return response;
        }

        //Get ALL Todos
        public async Task<Response> GetAllTasks()
        {
            Response response = new Response();
            try
            {
                var tasks = dbContext.Tasks.ToList();
                response.StatusCode = 200;
                response.ResponseMessage = "Okay!";
                response.ResultData = tasks;
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.ResponseMessage = ex.Message;
            }
            return response;
        }

        //Udate Todo status
        public async Task<Response> UpdatedTask(long id, TaskDto updatedTask)
        {
            Response response = new Response();
            try
            {
                var task = dbContext.Tasks.FirstOrDefault(t => t.Id == id);
                if(task == null)
                {
                    response.StatusCode = 404;
                }

                task.TaskTitle = updatedTask.TaskTitle;
                task.IsImportant = updatedTask.IsImportant;
                task.IsCompleted = updatedTask.IsCompleted;

                dbContext.Tasks.Update(task);
                dbContext.SaveChanges();
                response.StatusCode = 200;
                response.ResponseMessage = "Task Updated";
                response.ResultData = updatedTask;
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.ResponseMessage = ex.Message;
            }
            return response;
        }

        //Filter-out single Todo by Id
        public async Task<Response> FilterTaskById(long id)
        {
            Response response = new Response();
            try
            {
                var filterTask = dbContext.Tasks.FirstOrDefault(x => x.Id == id);

                response.StatusCode = 200;
                response.ResponseMessage = "Okay";
                response.ResultData = filterTask;
            }
            catch(Exception ex)
            {
                response.StatusCode = 500;
                response.ResponseMessage = ex.Message;
            }
            return response;
        }

        //Delete Todo by Id
        public async Task<Response> DeleteTaskById(long id)
        {
            Response response = new Response();
            try
            {
                var seletedTask = dbContext.Tasks.FirstOrDefault(x => x.Id == id);
                if(seletedTask == null)
                {
                    response.StatusCode = 404;
                    response.ResponseMessage = "Not Found!";
                    return response;
                }
                dbContext.Tasks.Remove(seletedTask);
                await dbContext.SaveChangesAsync();
                response.StatusCode = 200;
                response.ResponseMessage = "Success!";

            }
            catch(Exception ex)
            {
                response.StatusCode = 500;
                response.ResponseMessage = ex.Message;
            }
            return response;
        }

    //    public async Task<Response> GenrateJwtToken(User user)
    //    {
    //        Response response = new Response();

    //        try
    //        {
    //            // Validate input
    //            if (user == null || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.UserName))
    //            {
    //                response.StatusCode = 400;
    //                response.ResponseMessage = "Invalid user data. Ensure the user object contains valid details.";
    //                return response;
    //            }

    //            // Define claims for the token
    //            var claims = new[]
    //            {
    //        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
    //        new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
    //        new Claim(ClaimTypes.Email, user.Email)
    //    };

    //            // Create a key using the secret key
    //            var secretKey = "s3cRe7K3y@ToDoApp123!SuperSecretDummyKey"; // Replace with a securely stored key
    //            if (string.IsNullOrEmpty(secretKey))
    //            {
    //                response.StatusCode = 500;
    //                response.ResponseMessage = "The secret key is not configured.";
    //                return response;
    //            }

    //            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
    //            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    //            // Define the token
    //            var token = new JwtSecurityToken(
    //                issuer: "ToDoApp",
    //                audience: "ToDoAppUsers",
    //                claims: claims,
    //                expires: DateTime.UtcNow.AddMinutes(60), // Token validity
    //                signingCredentials: credentials
    //            );

    //            // Generate the token string
    //            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

    //            // Populate response object
    //            response.StatusCode = 200;
    //            response.ResponseMessage = "Token generated successfully.";
    //            response.ResultData = tokenString;
    //        }
    //        catch (Exception ex)
    //        {
    //            // Handle exceptions
    //            response.StatusCode = 500;
    //            response.ResponseMessage = $"An error occurred while generating the token: {ex.Message}";
    //        }

    //        return response;
    //    }
    }

    


}
