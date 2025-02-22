using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Domain.Models.Auth;
using Domain.Models.Management;
using Infrastructure.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Infrastructure.Services;

public class ChatHub : Hub
{
    private readonly MongoDBContext _context;
    private readonly IHttpContextAccessor _http;
    private readonly UserManager<User> _userManager;
    private readonly MessageMongoInteract _messagesRepository;

    public ChatHub(MongoDBContext context, IHttpContextAccessor http, UserManager<User> userManager)
    {
        _context = context;
        _http = http;
        _userManager = userManager;
        _messagesRepository = new MessageMongoInteract(context);
    }

    public async Task SendMessage(string receiverEmail, string message)
    {
        try
        {
            if (!_http.HttpContext.User.Identity.IsAuthenticated)
            {
                Console.WriteLine("User is not authenticated.");
                throw new Exception("User is not authenticated.");
            }

            var currentUserId = _http.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            var receiver = await _userManager.FindByEmailAsync(receiverEmail);

            if (currentUser == null)
            {
                Console.WriteLine("Current user is null.");
                throw new Exception("Current user not found.");
            }

            if (receiver == null)
            {
                Console.WriteLine("Receiver is null.");
                throw new Exception("Receiver not found.");
            }

            var chatMessage = new OMessage
            {
                UserId = currentUserId,
                User = currentUser,
                Message = message,
                Timestamp = DateTime.Now
            };

            await _messagesRepository.AddMessageAsync(chatMessage);

            await Clients.User(receiver.Id).SendAsync("ReceiveMessage", currentUser.Email, message);
            await Clients.User(currentUserId).SendAsync("ReceiveMessage", receiver.Email, message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred in SendMessage: {ex.Message}");
            throw;
        }
    }

    public async Task AdminSendMessage(string userId, string message)
    {
        try
        {
            var adminEmail = "nguyenbinh031104@gmail.com"; 
            var admin = await _userManager.FindByEmailAsync(adminEmail);

            if (admin == null)
            {
                Console.WriteLine("Admin user is null.");
                throw new Exception("Admin user not found.");
            }

            var chatMessage = new OMessage
            {
                UserId = admin.Id,
                User = admin,
                Message = message,
                Timestamp = DateTime.Now
            };

            await _messagesRepository.AddMessageAsync(chatMessage);

            await Clients.User(userId).SendAsync("ReceiveMessage", admin.Email, message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            throw;
        }
    }
}
