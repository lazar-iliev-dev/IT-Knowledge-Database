using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KnowledgeApi.Controllers;
using KnowledgeApi.DTOs;
using KnowledgeApi.Services;

namespace Tests;

public class ArticlesControllerTests
{
    private readonly Mock<IArticleService> _mockArticleService;
        private readonly Mock<ILogger<ArticlesController>> _mockLogger;
        private readonly ArticlesController _controller;

    public ArticlesControllerTests()
    {
        _mockArticleService = new Mock<IArticleService>();
        _mockLogger = new Mock<ILogger<ArticlesController>>();
        _controller = new ArticlesController(_mockArticleService.Object, _mockLogger.Object);
    }

    [Fact]
     public async Task GetAll_WithArticles_ReturnsOkResultWithArticles()
        {
            // Arrange
            var articles = new List<ArticleDto>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Test Article 1",
                    Content = "Content 1",
                    Category = "Programming",
                    Tags = new List<string> { "C#", "Backend" },
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Test Article 2",
                    Content = "Content 2",
                    Category = "Database",
                    Tags = new List<string> { "SQL" },
                    CreatedAt = DateTime.UtcNow
                }
            };
           _mockArticleService.Setup(s => s.GetAllArticlesAsync())
                .ReturnsAsync(articles);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedArticles = Assert.IsAssignableFrom<IEnumerable<ArticleDto>>(okResult.Value);
            Assert.Equal(2, returnedArticles.Count());
            _mockArticleService.Verify(s => s.GetAllArticlesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAll_WithNoArticles_ReturnsOkWithEmptyList()
        {
            // Arrange
            _mockArticleService.Setup(s => s.GetAllArticlesAsync())
                .ReturnsAsync(new List<ArticleDto>());

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedArticles = Assert.IsAssignableFrom<IEnumerable<ArticleDto>>(okResult.Value);
            Assert.Empty(returnedArticles);
        }

        [Fact]
        public async Task GetAll_WithServiceException_Returns500Error()
        {
            // Arrange
            _mockArticleService.Setup(s => s.GetAllArticlesAsync())
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetAll();

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        #region GetById Tests
        
        [Fact]
        public async Task GetById_WithValidId_ReturnsOkWithArticle()
        {
            // Arrange
            var articleId = Guid.NewGuid();
            var article = new ArticleDto
            {
                Id = articleId,
                Title = "Test Article",
                Content = "Test Content",
                Category = "Programming",
                Tags = new List<string> { "Test" },
                CreatedAt = DateTime.UtcNow
            };

            _mockArticleService.Setup(s => s.GetArticleByIdAsync(articleId))
                .ReturnsAsync(article);

            // Act
            var result = await _controller.GetById(articleId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedArticle = Assert.IsType<ArticleDto>(okResult.Value);
            Assert.Equal(articleId, returnedArticle.Id);
            Assert.Equal("Test Article", returnedArticle.Title);
        }

        [Fact]
        public async Task GetById_WithEmptyGuid_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetById(Guid.Empty);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task GetById_WithNonExistentId_ReturnsNotFound()
        {
            // Arrange
            var articleId = Guid.NewGuid();
            _mockArticleService.Setup(s => s.GetArticleByIdAsync(articleId))
                .ReturnsAsync((ArticleDto)null);

            // Act
            var result = await _controller.GetById(articleId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task GetById_WithServiceException_Returns500Error()
        {
            // Arrange
            var articleId = Guid.NewGuid();
            _mockArticleService.Setup(s => s.GetArticleByIdAsync(articleId))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetById(articleId);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        #endregion

        #region Create Tests

        [Fact]
        public async Task Create_WithValidData_ReturnsCreatedAtAction()
        {
            // Arrange
            var createDto = new CreateArticleDto
            {
                Title = "New Article",
                Content = "New Content",
                Category = ArticleCategoryDto.Programming,
                Tags = new List<string> { "Test", "Backend" }
            };

            var createdArticle = new ArticleDto
            {
                Id = Guid.NewGuid(),
                Title = createDto.Title,
                Content = createDto.Content,
                Category = createDto.Category.ToString(),
                Tags = createDto.Tags,
                CreatedAt = DateTime.UtcNow
            };

            _mockArticleService.Setup(s => s.CreateArticleAsync(It.IsAny<CreateArticleDto>()))
                .ReturnsAsync(createdArticle);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(ArticlesController.GetById), createdResult.ActionName);
            Assert.Equal(createdArticle.Id, ((ArticleDto)createdResult.Value).Id);
            _mockArticleService.Verify(s => s.CreateArticleAsync(It.IsAny<CreateArticleDto>()), Times.Once);
        }

        [Fact]
        public async Task Create_WithNullBody_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.Create(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task Create_WithEmptyTitle_ReturnsBadRequest()
        {
            // Arrange
            var createDto = new CreateArticleDto
            {
                Title = "",
                Content = "Valid content here",
                Category = ArticleCategoryDto.Programming
            };

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task Create_WithTitleTooLong_ReturnsBadRequest()
        {
            // Arrange
            var createDto = new CreateArticleDto
            {
                Title = new string('a', 501),
                Content = "Valid content",
                Category = ArticleCategoryDto.Programming
            };

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task Create_WithServiceException_Returns500Error()
        {
            // Arrange
            var createDto = new CreateArticleDto
            {
                Title = "Article",
                Content = "Content",
                Category = ArticleCategoryDto.Programming
            };

            _mockArticleService.Setup(s => s.CreateArticleAsync(It.IsAny<CreateArticleDto>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task Update_WithValidData_ReturnsNoContent()
        {
            // Arrange
            var articleId = Guid.NewGuid();
            var updateDto = new UpdateArticleDto
            {
                Title = "Updated Article",
                Content = "Updated Content",
                Category = ArticleCategoryDto.Database,
                Tags = new List<string> { "Updated" }
            };

            _mockArticleService.Setup(s => s.UpdateArticleAsync(articleId, It.IsAny<UpdateArticleDto>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Update(articleId, updateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockArticleService.Verify(s => s.UpdateArticleAsync(articleId, updateDto), Times.Once);
        }

        [Fact]
        public async Task Update_WithEmptyId_ReturnsBadRequest()
        {
            // Arrange
            var updateDto = new UpdateArticleDto
            {
                Title = "Updated",
                Content = "Content",
                Category = ArticleCategoryDto.Programming
            };

            // Act
            var result = await _controller.Update(Guid.Empty, updateDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task Update_WithNonExistentId_ReturnsNotFound()
        {
            // Arrange
            var articleId = Guid.NewGuid();
            var updateDto = new UpdateArticleDto
            {
                Title = "Updated",
                Content = "Content",
                Category = ArticleCategoryDto.Programming
            };

            _mockArticleService.Setup(s => s.UpdateArticleAsync(articleId, It.IsAny<UpdateArticleDto>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Update(articleId, updateDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task Delete_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var articleId = Guid.NewGuid();
            _mockArticleService.Setup(s => s.DeleteArticleAsync(articleId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(articleId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockArticleService.Verify(s => s.DeleteArticleAsync(articleId), Times.Once);
        }

        [Fact]
        public async Task Delete_WithEmptyId_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.Delete(Guid.Empty);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task Delete_WithNonExistentId_ReturnsNotFound()
        {
            // Arrange
            var articleId = Guid.NewGuid();
            _mockArticleService.Setup(s => s.DeleteArticleAsync(articleId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(articleId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task Delete_WithServiceException_Returns500Error()
        {
            // Arrange
            var articleId = Guid.NewGuid();
            _mockArticleService.Setup(s => s.DeleteArticleAsync(articleId))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.Delete(articleId);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        #endregion
    }