# URL Shortener

Application that shortens any URL and has possibility to navigate by this short equivalent

---

## Table of Contents

- [Installation](#installation)
- [Environment Configuration](#environment-configuration)

---

## Installation

### Step 1: **Clone the repository**
   ```
   git clone https://github.com/Kurulko/URL-Shortener.git
   cd URL_Shortener
  ```
### Step 2: **Install Back-End Dependencies**

#### 1. Navigate to the webapi directory:
```
cd webapi
```
#### 2. Restore the .NET dependencies:
```
dotnet restore
```
### Step 3. Install Front-End Dependencies:
#### 1. Navigate to the angularapp directory:
```
cd ../angularapp
```
#### 2. Install the necessary dependencies for the Angular project:
```
npm install
```

---

## Environment Configuration

### Back-end (ASP.NET)
Modify settings in `webapi/appsettings.json`

#### Configure appsettings.json

##### 1. In the webapi project, locate or create the appsettings.json file in the root folder.
##### 2. Add your sensitive settings (e.g., connection strings, JWT settings) to appsettings.json:

```
{
  ... ,
  "ConnectionStrings": {
    "URLShortenerConnection": "Server=(localdb)\\mssqllocaldb; Database=URLShortener; MultipleActiveResultSets=True;"
  },

  "JwtSettings": {
    "SecretKey": "your-secret-key",
    "Issuer": "your-issuer",
    "Audience": "your-audience",
    "ExpirationDays": 7
  },

  "Admin": {
    "Name": "YourName",
    "Password": "your-password"
  }
}

```

### Front-end (Angular)

#### Configure Environment Variables

##### 1. Modify `angularapp/src/environments/environment.prod.ts`:
```
export const environment = {
  production: true,
  apiUrl: 'https://your-production-api-url/api'
};
```

##### 2. Modify `angularapp/src/proxy.conf.js`:
```
const PROXY_CONFIG = [
  {
    context: [
      "/api",
      "/resources"
    ],
    target: "https://your-production-api-url/api",
    secure: false,
  }
]

module.exports = PROXY_CONFIG;
```

## Running

### Back-End (ASP.NET Core)

#### 1. Navigate to the webapi directory and run the back-end:
```
cd webapi
dotnet run
```
This will start your back-end server

### Front-End (Angular)

#### 1. Navigate to the angularapp directory and run the Angular application:
```
cd ../angularapp
ng serve
```
The Angular application will be available at `http://localhost:4200`.
