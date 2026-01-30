# MandaA Marketplace – Full-Stack E-Commerce Application

A full-stack e-commerce web application inspired by Amazon. The application supports product browsing, fuzzy search, cart management, secure checkout with Stripe, and detailed order tracking.

The project is built with a React frontend and an ASP.NET Core backend, using Entity Framework Core and MySQL for data persistence.

Visit the demo at [https://masonengland.online](https://masonengland.online)

---

## Features

- Product browsing with detailed item pages
- Fuzzy search for flexible product discovery
- Add to cart and cart management
- Secure checkout flow using Stripe
- Order history with detailed order views
- RESTful backend API

---

## Tech Stack

### Frontend
- React (JavaScript, no TypeScript)
- Vite

### Backend
- ASP.NET Core Web API
- Entity Framework Core
- JWTs
- Bcrypt
- MySQL
- Docker
- NGINX

### Payments
- [Stripe API](https://stripe.com/)

### dummy data
- [Fakestore API](https://fakestoreapi.com/)

---

## Installation and Setup

### Prerequisites
- Stripe account
- Docker
- Docker Compose

---

## Environment Variables

Before running the application, configure the following environment variables:

Variable Name: ACCESS_TOKEN_SECRET  
Description: Secret key used for HMACSHA256 password hashing and token generation

Variable Name: STRIPE_API_KEY  
Description: Stripe secret API key used for payment processing

Variable Name: APPLICATION_URL  
Description: URL of the frontend application (Vite development server address)

Variable Name: DB_ROOT_PASSWORD
Description: A password of your choice for the docker mysql instace

Variable Name: DB_CONNECTION_STRING
Description: The connection string for the database, "Server=db;Port=3306;Database=MandAMarketplace;User=root;Password=(same as DB_ROOT_PASSWORD);

Example values:
- APPLICATION_URL=http://localhost:5173
- APPLICATION_URL=http://localhost:3000

---

### Setting Environment Variables

create a .env file in the /server directory and place all of the environment variables there. They will be read by docker and automatically inserted

---

## App Setup

The backend can be easily run with docker:
    cd server
    docker compose up -d --build

ensure you have docker installed and the docker daemon running. 

If changes are made to the frontend you can build the react code again
    npm run build
    docker compose down
    docker compose up -d --build

you can also use the vite dev server when making changes to get access to features like hot reload

---

## Usage

1. creat a .env file with the necessary environment variables
2. run $docker compose up -d --build
3. visit at https://localhost
4. to turn off run $docker compose down


---

## How It Works

The React frontend handles UI rendering and user interaction. The ASP.NET Core API exposes endpoints for products, carts, orders, and checkout. Entity Framework Core manages database access and relationships. MySQL stores product data and order history. Stripe handles secure payment processing so that payments can remain PCI compiant. Docker for containerization. NGINX for reverse proxy and scalable request handling

---

## Known Limitations

- No admin interface for product management
- Client-side rendered frontend only
- lack of loading indicators
- self signed ssl certificate 

---

## Future Improvements

- Admin dashboard for inventory management
- Product reviews and ratings
- multiple photos of items
- Performance optimizations
- Improved mobile responsiveness
- lets encrpyt signed ssl certificate

---

## License

MIT License

---

## Author

Mason England
