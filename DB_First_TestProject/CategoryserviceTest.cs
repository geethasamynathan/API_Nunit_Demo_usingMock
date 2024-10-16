using DB_First_Demo.Models;
using DB_First_Demo.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_First_TestProject
{
    [TestFixture]
    internal class CategoryserviceTest
    {
        private Mock<BikeStoresContext> _mockContext;
        private CatergoryService _categoryService;
        private List<Category> _categories;

        [SetUp]
        public void Setup()
        {
            _categories = new List<Category>
            {
                new Category {CategoryId=1,CategoryName="Electric Bike"},
                new Category{CategoryId=2,CategoryName="Mountain Bike"}
            };
            var mockCategorySet = new Mock<DbSet<Category>>();
            mockCategorySet.As<IQueryable<Category>>().Setup(m => m.Provider).Returns(_categories.AsQueryable().Provider);
            mockCategorySet.As<IQueryable<Category>>().Setup(m => m.Expression).Returns(_categories.AsQueryable().Expression);
            mockCategorySet.As<IQueryable<Category>>().Setup(m => m.ElementType).Returns(_categories.AsQueryable().ElementType);
            mockCategorySet.As<IQueryable<Category>>().Setup(m => m.GetEnumerator()).Returns(_categories.AsQueryable().GetEnumerator());


            _mockContext = new Mock<BikeStoresContext>();
            _mockContext.Setup(c => c.Categories).Returns(mockCategorySet.Object);
            _categoryService = new CatergoryService(_mockContext.Object);

        }



        [Test]
        public void GetCategories_ShouldReturnAllCategories()
        {
            var categories = _categoryService.GetCategories();
            Assert.That(categories.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetCategoryById_ShouldReturnCorrectCategory()
        {
            var category = _categoryService.GetCategoryById(1);
            Assert.NotNull(category);
            Assert.That(category.CategoryName, Is.EqualTo("Electric Bike"));
        }

        [Test]
        public void AddNewCategory_ShouldAddCategory()
        {
            var newCategory = new Category { CategoryId = 3, CategoryName = "Hybrid Bikes" };
            _categoryService.AddNewCategory(newCategory);

            _mockContext.Verify(m => m.Categories.Add(It.IsAny<Category>()), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());

        }
        [Test]
        public void DeleteCategory_ShouldRemoveCategory()
        {
            
            // Act
            var result = _categoryService.DeleteCategory(1);

            // Assert
            Assert.That(result, Is.EqualTo("Category removed ."));
            _mockContext.Verify(m => m.Categories.Remove(It.IsAny<Category>()), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Test]
        public void DeleteCategory_ShouldReturnError_WhenCategoryNotFound()
        {
            // Act
            var result = _categoryService.DeleteCategory(3);

            // Assert
            Assert.That(result, Is.EqualTo("Error while remove the category"));
            _mockContext.Verify(m => m.Categories.Remove(It.IsAny<Category>()), Times.Never());
            _mockContext.Verify(m => m.SaveChanges(), Times.Never());

        }
        [Test]
        public void UpdateCategory_ShouldUpdateCategory_WhenCategoryExists()
        {
            // Arrange
            var updateCategory = new Category { CategoryId = 1, CategoryName = "Updated Name" };
            var category = new Category { CategoryId = 1, CategoryName = "Old Name" };

             // Act
            var result = _categoryService.UpdateCategory(updateCategory);

            // Assert
            Assert.NotNull(result);
            Assert.That(result.CategoryName, Is.EqualTo("Updated Name"));
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Test]
        public void UpdateCategory_ShouldReturnNull_WhenCategoryDoesNotExist()
        {
            // Arrange
            var updateCategory = new Category { CategoryId = 3, CategoryName = "New Name" };
            // Act
            var result = _categoryService.UpdateCategory(updateCategory);

            // Assert
            Assert.IsNull(result);
            _mockContext.Verify(m => m.SaveChanges(), Times.Never());
        }
    }
    }
