using System;
using Unity;
using Unity.Lifetime;
using FashionStore.Data;
using FashionStore.Repositories.Interfaces;
using FashionStore.Repositories.Implementations;
using FashionStore.Services.Interfaces;
using FashionStore.Services.Implementations;

namespace FashionStore
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              RegisterTypes(container);
              return container;
          });

        /// <summary>
        /// Configured Unity Container.
        /// </summary>
        public static IUnityContainer Container => container.Value;
        #endregion

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The unity container to configure.</param>
        public static void RegisterTypes(IUnityContainer container)
        {
            // Register DbContext
            container.RegisterType<ApplicationDbContext>(new HierarchicalLifetimeManager());

            // Register Unit of Work
            container.RegisterType<IUnitOfWork, UnitOfWork>(new HierarchicalLifetimeManager());

            // Register Repositories
            container.RegisterType<IProductRepository, ProductRepository>();
            container.RegisterType<ICategoryRepository, CategoryRepository>();
            container.RegisterType<IOrderRepository, OrderRepository>();
            container.RegisterType<IOrderDetailRepository, OrderDetailRepository>();
            container.RegisterType<IUserRepository, UserRepository>();
            container.RegisterType<ICartRepository, CartRepository>();
            container.RegisterType<ICartItemRepository, CartItemRepository>();
            container.RegisterType<IRoleRepository, RoleRepository>();

            // Register Services
            container.RegisterType<IProductService, ProductService>();
            container.RegisterType<ICategoryService, CategoryService>();
            container.RegisterType<IUserService, UserService>();
            container.RegisterType<ICartService, CartService>();
            container.RegisterType<IOrderService, OrderService>();
            container.RegisterType<IEmailService, EmailService>();
        }
    }
}