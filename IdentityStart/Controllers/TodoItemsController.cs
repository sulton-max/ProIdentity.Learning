using System.Security.Claims;
using IdentityStart.Data.DataContexts;
using IdentityStart.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityStart.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TodoItemsController : ControllerBase
{
    private readonly AppDbContext _appDbContext;

    public TodoItemsController(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var result = await _appDbContext.TodoItems.ToListAsync();
        return result.Any() ? Ok(result) : BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] TodoItem model)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        model.OwnerId = userId;
        var result = await _appDbContext.TodoItems.AddAsync(model);
        await _appDbContext.SaveChangesAsync();

        var url = Url.Action(nameof(GetById),
            new
            {
                model.Id
            });
        return Created(url!, result.Entity);
    }

    [HttpPut]
    public async Task<IActionResult> Put([FromBody] TodoItem model)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var todoItem = await _appDbContext.TodoItems.FirstOrDefaultAsync(x => x.Id == model.Id && x.OwnerId == userId);

        if (todoItem is null)
            return NotFound();

        if (todoItem.OwnerId != userId)
            return Unauthorized();

        todoItem.Task = model.Task;
        todoItem.Complete = model.Complete;
        await _appDbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete([FromRoute] long id)
    {
        var foundTodoItem = await _appDbContext.TodoItems.FirstOrDefaultAsync(x => x.Id == id);

        if (foundTodoItem is null)
            return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (foundTodoItem.OwnerId != userId)
            return Unauthorized();

        _appDbContext.TodoItems.Remove(foundTodoItem);
        await _appDbContext.SaveChangesAsync();

        return NoContent();
    }
}