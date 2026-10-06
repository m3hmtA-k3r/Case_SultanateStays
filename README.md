# SultanateStays

A hotel search application built with ASP.NET Core 8 MVC on top of the Booking.com API (via RapidAPI): search a city, compare hotels, and open a hotel's details and photos.

<img width="1576" height="844" alt="image" src="https://github.com/user-attachments/assets/eb8a69cd-f3f3-4665-a2d1-a55b7bc046b2" />

This project is based on a case study from the training I received at M&Y Yazılım Eğitim Akademi, under the guidance of Murat Yücedağ and Erhan Gündüz. The case asked for an API integration project; I wrote the code myself and built the interface with Tailwind CSS.

## Features

- __Search form:__ city, check-in and check-out dates, adults, children with their ages, number of rooms and currency
- __Results:__ hotel cards with name, photo, price for the selected nights and review score
- __Hotel details:__ address, facilities, description and a photo gallery
- __Validation before any API call:__
  - check-in cannot be in the past
  - check-out must be after check-in
  - every child needs an age between 0 and 17
- __Clear error states:__
  - "city not found" when the destination search returns nothing
  - a friendly message when the API is unavailable
  - a general error page for anything unexpected

## Tech Stack

- __Framework:__ ASP.NET Core 8 MVC
- __API:__ Booking.com API on RapidAPI, called with `IHttpClientFactory`
- __Configuration:__ Options pattern (`RapidApiOptions`) and User Secrets for the API key
- __Styling:__ Tailwind CSS v4, built with the Tailwind CLI
- __Scripts:__ a small vanilla JavaScript file for the search form (child age fields and date limits)

There is no database: every result comes live from the API.

## Architecture

```
Controllers/HotelsController   search and details actions
Models/                        search query and the app's own hotel models
Models/RapidApi/               response models that mirror the API's JSON
Options/RapidApiOptions        base URL, host and key, bound from configuration
ViewModels/                    page models for search and details
```

A search runs in two steps. The city name is first sent to `searchDestination` to get a destination id, which is then used in `searchHotels`. The details page combines three calls: `getHotelDetails`, `getDescriptionAndInfo` and `getHotelPhotos`.

API responses are mapped to the application's own models (`HotelSummary`, `HotelDetails`) before they reach the views, so a change in the API's JSON stays in one place. Every request accepts a `CancellationToken`, so leaving the page stops the pending API call.

## Getting Started

### Prerequisites

- .NET 8 SDK
- Node.js (only to rebuild the stylesheet; the built CSS is in the repository)
- A RapidAPI account subscribed to the Booking.com API (`booking-com15`)

### Setup

```bash
git clone https://github.com/m3hmtA-k3r/Case_SultanateStays.git
cd Case_SultanateStays/SultanateStays
dotnet user-secrets set "RapidApi:ApiKey" "<your RapidAPI key>"
dotnet run
```

The key is read from User Secrets; `appsettings.json` keeps the `ApiKey` field empty on purpose.

To change the styles:

```bash
npm install
npm run watch:css
```

## What I learned

- __An API key never goes into the repository.__ The key lives in User Secrets and is bound through the Options pattern. `appsettings.json` only shows the shape of the settings, so the project can be shared publicly without leaking the key.
- __Build against saved responses first.__ The API has a request quota. While building the pages, I worked from saved JSON responses and switched to live calls only when the screens were ready. This kept the quota for real testing.
- __Not every JSON field has a fixed type.__ The `message` field in the API's responses does not always have the same shape, so binding it to a `string` broke deserialization. I typed it as `JsonElement` and read only the fields the application needs.
- __Validate before you spend a request.__ Date and child-age checks run before any API call, so an invalid search costs nothing and shows the user exactly what to fix.

## Status

Complete. Search, results and hotel details work against the live API.

## Acknowledgements

Thanks to Murat Yücedağ and Erhan Gündüz at M&Y Yazılım Eğitim Akademi for the training and the case study this project grew out of.

## About me

Mehmet Asker, a self-taught full stack developer with a background in operations management. More projects: [github.com/m3hmtA-k3r](https://github.com/m3hmtA-k3r)
