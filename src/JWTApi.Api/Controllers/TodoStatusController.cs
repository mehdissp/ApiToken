using JWTApi.Api.Response;
using JWTApi.Api.ViewModels;
using JWTApi.Api.ViewModels.Project;
using JWTApi.Api.ViewModels.TodoStatus;
using JWTApi.Application.Services;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;

namespace JWTApi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TodoStatusController : ControllerBase
    {
        private readonly TodoStatusService _todoStatus;
        private readonly TodoService _todo;
        public TodoStatusController(TodoStatusService todoStatusService, TodoService todoService)
        {
            _todoStatus = todoStatusService;
            _todo = todoService;
        }

        [HttpGet("GetTodoStatus")]
        public async Task<IActionResult> GetTodoStatus([FromQuery] int projectId, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var roleId = User.Claims.FirstOrDefault(c => c.Type == "roleId")?.Value;
            var (result, deletePerm, editPerm, insertPerm, viewPerm) = await _todoStatus.GetTodoStatus(projectId, roleId, cancellationToken);
            var todo = await _todo.TodoWithTagsViewsAsync(userId, roleId, cancellationToken);
            var results = new
            {
                Access = new
                {
                    deleteTodoStatus = deletePerm,
                    editTodoStatus = editPerm,
                    insertTodoStatus = insertPerm,
                    viewTodoStatus = viewPerm,
                },
                Columns = result.Select(s => new
                {
                    Id = s.Id,
                    Title = s.Name,
                    Color = s.Color,
                    OrderNum = s.OrderNum,
                    deleteTodoStatus = s.DeleteTodoStatus,
                    editTodoStatus = s.EditTodoStatus,
                    insertTodoStatus = s.InsertTodoStatus,
                    viewTodoStatus = s.ViewTodoStatus,
                    Tasks = todo.Where(t => t.StatusId == s.Id).Select(t => new
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Description = t.Description,
                        Priority = t.Priority,
                        Assignee = t.UserNameTodo,
                        DueDate = t.DueDate,
                        CreatedAt = t.CreatedAt,
                        UserNameCreator = t.UserNameCreator,
                        UserIdTodo=t.UserIdTodo,
                        Tags = t.Tags,
                        CountComment=t.CountComment,
                        IsOverdute=t.IsOverdute,
                        deleteButton=t.DeleteButton,
                        editButton=t.EditButton,
                        //deleteTodoStatus=t.DeleteTodoStatus,
                        //editTodoStatus = t.EditTodoStatus,
                        //insertTodoStatus = t.InsertTodoStatus,
                        //viewTodoStatus = t.ViewTodoStatus,

                    })
                }).OrderBy(c => c.OrderNum)
              
            };


            return ResponseApi.Ok(results).ToHttpResponse();

        }

        [HttpPost("InsertTodoStatus")]
        public async Task<IActionResult> InsertTodoStatus([FromBody] TodoStatusViewModel todoStatusViewModel, CancellationToken cancellationToken)
        {
            if (!todoStatusViewModel.Color.StartsWith("#") && !string.IsNullOrEmpty(todoStatusViewModel.Color))
            {
                todoStatusViewModel.Color = "#" + todoStatusViewModel.Color;
            }
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            await _todoStatus.InsertTodoStatus(todoStatusViewModel.ProjectId, todoStatusViewModel.Name
                , todoStatusViewModel.Color, cancellationToken);

            return ResponseApi.Ok().ToHttpResponse();

        }



        [HttpPost("UpdateTodoStatus")]
        public async Task<IActionResult> UpdateTodoStatus([FromBody] TodoStatusViewModel todoStatusViewModel, CancellationToken cancellationToken)
        {
            if (!todoStatusViewModel.Color.StartsWith("#") && !string.IsNullOrEmpty(todoStatusViewModel.Color))
            {
                todoStatusViewModel.Color = "#" + todoStatusViewModel.Color;
            }
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            await _todoStatus.UpdateTodoStatus(todoStatusViewModel.Id, todoStatusViewModel.Name
                , todoStatusViewModel.Color,todoStatusViewModel.OrderNum, cancellationToken);

            return ResponseApi.Ok().ToHttpResponse();

        }

        [HttpDelete("DeleteTodoStatus")]
        public async Task<IActionResult> DeleteTodoStatus([FromQuery] int id, CancellationToken cancellationToken)
        {
      
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            await _todoStatus.DeleteTodoStatus(id, cancellationToken);

            return ResponseApi.Ok().ToHttpResponse();

        }

        [HttpGet("GetTags")]
        public async Task<IActionResult> GetTags([FromQuery] int projectId, CancellationToken cancellationToken)
        {
            //var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var result = await _todoStatus.GetTagsAsync(projectId, cancellationToken);

            return ResponseApi.Ok(result).ToHttpResponse();

        }


    }
}
