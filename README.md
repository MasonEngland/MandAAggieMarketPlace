# MandaA Marketplace – Full-Stack E-Commerce Application

A full-stack e-commerce web application inspired by Amazon. The application supports product browsing, fuzzy search, cart management, secure checkout with Stripe, and detailed order tracking.

The project is built with a React frontend and an ASP.NET Core backend, using Entity Framework Core and MySQL for data persistence.

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

### Payments
- Stripe API

---

## Installation and Setup

### Prerequisites
- Node.js
- .NET SDK
- MySQL
- Stripe account

---

## Environment Variables

Before running the application, configure the following environment variables:

Variable Name: ACCESS_TOKEN_SECRET  
Description: Secret key used for HMACSHA256 password hashing and token generation

Variable Name: STRIPE_API_KEY  
Description: Stripe secret API key used for payment processing

Variable Name: APPLICATION_URL  
Description: URL of the frontend application (Vite development server address)

Example values:
- APPLICATION_URL=http://localhost:5173
- APPLICATION_URL=http://localhost:3000

---

### Setting Environment Variables

Linux / macOS:

    export ACCESS_TOKEN_SECRET=your_secret_here
    export STRIPE_API_KEY=your_stripe_key_here
    export APPLICATION_URL=http://localhost:5173

Windows (PowerShell):

    $env:ACCESS_TOKEN_SECRET="your_secret_here"
    $env:STRIPE_API_KEY="your_stripe_key_here"
    $env:APPLICATION_URL="http://localhost:5173"

---

## Backend Setup

The backend runs like a standard ASP.NET Core application.

    cd backend
    dotnet restore
    dotnet ef database update
    dotnet run

If using Entity Framework migrations:

    dotnet ef database update

---

## Frontend Setup

    cd frontend
    npm install
    npm run dev

The frontend will be available at http://localhost:5173 by default.

---

## Usage

1. Start the ASP.NET Core backend
2. Start the React frontend
3. Open the frontend in a browser
4. Browse products or use fuzzy search
5. Add items to the cart and complete checkout
6. View order history and detailed order information


---

## How It Works

The React frontend handles UI rendering and user interaction. The ASP.NET Core API exposes endpoints for products, carts, orders, and checkout. Entity Framework Core manages database access and relationships. MySQL stores product data and order history. Stripe handles secure payment processing so that payments can remain PCI compiant.

---

## Known Limitations

- No admin interface for product management
- Client-side rendered frontend only
- lack of loading indicators

---

## Future Improvements

- Admin dashboard for inventory management
- Product reviews and ratings
- multiple photos of items
- Performance optimizations
- Improved mobile responsiveness

---

## License

MIT License

---

## Author

Mason England
