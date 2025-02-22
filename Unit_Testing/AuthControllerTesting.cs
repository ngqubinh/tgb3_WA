using System.Security.Authentication;
using Application.DTOs.Request.Authentication;
using Application.DTOs.Response;
using Application.Interfaces.Authentication;
using Application.ViewModels.Auth;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ShelkovyPut_Main.Controllers.Auth;

namespace Unit_Testing
{
    [TestFixture]
    public class AuthControllerTests
    {
        private Mock<IAuthService> _mockAuthService;
        private AuthController _authController;

        // Hàm SetUp này sẽ chạy trước mỗi bài kiểm tra, đảm bảo rằng _mockAuthService và _authController
        // luôn được khởi tạo mới và sẵn sàng cho test.
        // Ý là nó có cách hoạt động như constsructor á.
        // Còn nếu bí quá hay không thuộc bài thì trả lời là tài liệu hướng dẫn của Microsoft chỉ viết vậy.
        [SetUp]
        public void SetUp()
        {
            _mockAuthService = new Mock<IAuthService>();
            _authController = new AuthController(_mockAuthService.Object);
        }

        [Test]
        public async Task Login_ReturnsOkResult_WithValidCredentials()
        {
            // Arrange
            var loginRequest = new LoginRequest { Email = "testuser@example.com", Password = "password123" };
            var userResponse = new UserResponse { Message = "kokok" };
            _mockAuthService.Setup(service => service.LoginAsync(loginRequest)).ReturnsAsync(userResponse);

            // Act
            var result = await _authController.Login(loginRequest);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OkObjectResult>(result.Result);

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var actualResponse = okResult.Value as UserResponse;
            Assert.IsNotNull(actualResponse);
            Assert.AreEqual(userResponse.Message, actualResponse.Message);
            
            Console.WriteLine(actualResponse.Message);
        }

        [Test]
        public async Task Login_WithoutPassword()
        {
            // Arrange
            var loginRequest = new LoginRequest { Email = "test@gmail.com", Password = string.Empty };
            var passwordError = new InvalidCredentialException("Password cannot be empty");
            _mockAuthService.Setup(service => service.LoginAsync(loginRequest)).ThrowsAsync(passwordError);

            // Act
            var result = await _authController.Login(loginRequest);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);

            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);

            var actualResponse = badRequestResult.Value as UserResponse;
            Assert.IsNotNull(actualResponse);
            Assert.AreEqual(passwordError.Message, actualResponse.Message);

            // Print response to console
            Console.WriteLine(actualResponse.Message);
        }

        [Test]
        public async Task SignUp_WithValidCredentials()
        {
            SignupRequest request = new SignupRequest
            {
                Email = "test@gmail.com",
                Password = "test",
                ConfirmPassword = "test"
            };

            GeneralResponse response = new GeneralResponse(true, "Register successfully");
            this._mockAuthService.Setup(s => s.CreateAsync(request)).ReturnsAsync(response);

            ActionResult<GeneralResponse> result = await this._authController.Register(request);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OkObjectResult>(result.Result);

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);

            GeneralResponse actualResponse = okResult.Value as GeneralResponse;
            Assert.IsNotNull(actualResponse);
            Assert.AreEqual(response.Success, actualResponse.Success);
            Assert.AreEqual(response.Message, actualResponse.Message);

            Console.WriteLine($"Kết quả: {response.Success}, Nội dụng: {response.Message}");
        }

        [Test]
        public async Task SignUp_WithoutPassword()
        {
            SignupRequest request = new SignupRequest
            {
                Email = "test@gmail.com",
                Password = string.Empty,
                ConfirmPassword = string.Empty
            };

            GeneralResponse response = new GeneralResponse(false, "The password is needed");
            this._mockAuthService.Setup(s => s.CreateAsync(request)).ReturnsAsync(response);

            ActionResult<GeneralResponse> result = await this._authController.Register(request);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);

            var badResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badResult);

            GeneralResponse actualResponse = badResult.Value as GeneralResponse;
            Assert.IsNotNull(actualResponse);
            Assert.AreEqual(response.Message, actualResponse.Message);

            Console.WriteLine(actualResponse.Message);
        }

        [Test]
        public async Task SignUp_WithExistedEmail()
        {
            // Đầu vào
            string existedEmail = "nguyenquocbinh214@gmail.com";
            SignupRequest request = new SignupRequest
            {
                Email = existedEmail,
                Password = "test",
                ConfirmPassword = "test"
            };

            // Kỳ vọng
            GeneralResponse response = new GeneralResponse(false, $"{request.Email} is already existed");
            this._mockAuthService.Setup(s => s.CreateAsync(request)).ReturnsAsync(response);

            // Gọi phương thức Register từ AuthController á nghen
            // Nếu k biết AuthController ở đầu thì nó là Main -> Auth -> AuthController
            ActionResult<GeneralResponse> result = await this._authController.Register(request);

            // Kiểm tra liệu có null hay không
            // Thầy hỏi tại sao phải kiểm tra thì trả lời là giúp bắt lỗi dễ hơn thui
            Assert.IsNotNull(result);

            // Này là kiểm tra đầu vào có thiếu cái gì không á
            // Nếu đầu là 5 mà m chỉ cho 4 là hiện cái này á nghen
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
            var badResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badResult);

            // Này là kết quả trả về nè
            GeneralResponse actualResponse = badResult.Value as GeneralResponse;
            Assert.IsNotNull(actualResponse);
            Assert.AreEqual(response.Message, actualResponse.Message);

            Console.WriteLine(actualResponse.Message);
        }

        [Test]
        public async Task SignUp_WithoutConfirmPassword()
        {
            // Đầu vào
            SignupRequest request = new SignupRequest
            {
                Email = "test@gmail.com",
                Password = "Password123",
                ConfirmPassword = string.Empty
            };

            // Kỳ vọng kết quả trả về là cái này
            GeneralResponse response = new GeneralResponse(false, "The ConfirmPassword field is required.");
            this._mockAuthService.Setup(s => s.CreateAsync(request)).ReturnsAsync(response);

            // Gọi phương thức Register từ AuthController á nghen
            // Nếu k biết AuthController ở đầu thì nó là Main -> Auth -> AuthController
            ActionResult<GeneralResponse> result = await this._authController.Register(request);

            // Kiểm tra liệu có null hay không
            // Thầy hỏi tại sao phải kiểm tra thì trả lời là giúp bắt lỗi dễ hơn thui
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);

            // Này là kiểm tra đầu vào có thiếu cái gì không á
            // Nếu đầu là 5 mà m chỉ cho 4 là hiện cái này á nghen
            var badResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badResult);

            // Này là kết quả trả về nè
            GeneralResponse actualResponse = badResult.Value as GeneralResponse;
            Assert.IsNotNull(actualResponse);
            Assert.AreEqual(response.Message, actualResponse.Message);

            Console.WriteLine(actualResponse.Message);
        }

        [Test]
        public async Task SignUp_WithPasswordMismatch()
        {
            // Đầu vào
            SignupRequest request = new SignupRequest
            {
                Email = "test@gmail.com",
                Password = "Password123",
                ConfirmPassword = "DifferentPassword123"
            };

            // Kỳ vọng kết quả trả về là cái này
            GeneralResponse response = new GeneralResponse(false, "'ConfirmPassword' and 'Password' do not match.");
            this._mockAuthService.Setup(s => s.CreateAsync(request)).ReturnsAsync(response);

            // Gọi phương thức Register từ AuthController á nghen
            // Nếu k biết AuthController ở đầu thì nó là Main -> Auth -> AuthController
            ActionResult<GeneralResponse> result = await this._authController.Register(request);

            // Kiểm tra liệu có null hay không
            // Thầy hỏi tại sao phải kiểm tra thì trả lời là giúp bắt lỗi dễ hơn thui
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);

            // Này là kiểm tra đầu vào có thiếu cái gì không á
            // Nếu đầu là 5 mà m chỉ cho 4 là hiện cái này á nghen
            var badResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badResult);

            // Này là kết quả trả về nè
            GeneralResponse actualResponse = badResult.Value as GeneralResponse;
            Assert.IsNotNull(actualResponse);
            Assert.AreEqual(response.Message, actualResponse.Message);

            Console.WriteLine(actualResponse.Message);
        }

        [Test]
        public async Task SignUp_WithPasswordWithoutSpecialCharacter()
        {
            SignupRequest request = new SignupRequest
            {
                Email = "test@gmail.com",
                Password = "Password123", // Mật khẩu không có ký tự đặc biệt
                ConfirmPassword = "Password123"
            };

            GeneralResponse response = new GeneralResponse(false, "Password must include at least one letter, one number, and one special character.");
            this._mockAuthService.Setup(s => s.CreateAsync(request)).ReturnsAsync(response);

            ActionResult<GeneralResponse> result = await this._authController.Register(request);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);

            var badResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badResult);

            GeneralResponse actualResponse = badResult.Value as GeneralResponse;
            Assert.IsNotNull(actualResponse);
            Assert.AreEqual(response.Message, actualResponse.Message);

            Console.WriteLine(actualResponse.Message);
        }
    }
}
