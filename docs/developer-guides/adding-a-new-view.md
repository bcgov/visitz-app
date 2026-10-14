**ℹ️ Read this document fully before following its steps.**

# Adding a new view

This document covers how to add new views to the [Visitz project](../../visitz/Visitz).

## Will the new view show data from an external API?

If no, skip to the next section.

If yes, you'll need to check a few places to see if API support is already implemented in the app:

- [`Visitz.Services`](../../visitz/Visitz/Services)
- [`VisitzApi`](../../visitz/VisitzApi)
- [`VisitzModel`](../../visitz/VisitzModel)

If relevant services, endpoints, or models are not made yet you will need to create them. Refer to [Adding API support for data in Visitz](./adding-api-support.md), then come back here after implementing everything.

For display in UI: if database models already exist, you might not need to modify them—instead, try wrapping them with ViewModels for UI-specific logic.

## Creating a new view

### Use templates

It's more convenient to create new views using templates:

#### Visual Studio

> Right-click folder where you want to create a view > Add > New Item... > Left pane > Installed > C# Items > .NET MAUI

Choose applicable template, will usually be `.NET MAUI ContentView (XAML)`.

#### Terminal

To create a `.NET MAUI ContentView (XAML)` from CLI:

```powershell
dotnet new maui-view-xaml -o <path to create view in>
```

You can view all templates available for use with:

```powershell
dotnet new list
```

Use the short name with `dotnet new`.

### Where to create new views

This project uses feature-scoped namespacing to organize its views.

Most views go in [`Visitz.Views`](../../visitz/Visitz/Views) (some generic views/controls go in [`Visitz.Controls`](../../visitz/Visitz/Controls).)

Views for entity subsections (like Details, Notes, Attachments, etc.) go in [`Visitz.Views.Entity`](../../visitz/Visitz/Views/Entity) in their own sub-namespaces.

### Prefer ContentViews over ContentPages

Visitz uses only a few ContentPages, as it is easier to build complex UI/UX using ContentViews. But if a ContentPage makes the most sense for a feature, derive from `VisitzPage`.

With that in mind, most new views should derive from `ContentView` or one of the Visitz-specific base types:

#### `BaseContentView` 

Anything can be used for its `BindingContext`, view model or otherwise. Use this if you want basic lifecycle and logging support.

#### `ViewModelContentView<TViewModel> (inherits BaseContentView)`

Where `TViewModel` is `VisitzViewModel` or more derived, and its `BindingContext` should be set to that view model in its constructor.

Use this when you have a dedicated ViewModel paired with this ContentView.

#### `IcmRecordContentView<TViewModel> (inherits ViewModelContentView<TViewModel>)`

Where `TViewModel` is `IcmRecordViewModel` or more derived, and its `BindingContext` should be set to that view model in its constructor.

Use this when your ContentView is dedicated to showing information about a single record from the database.

> ℹ️ All of the sections displayed in `EntityView` use this type of ContentView.

### Prefer VisitzViewModel (or derived)

`VisitzViewModel` is lifecycle-aware, has logging built in, and some of its derived types have some degree of error handling.

### Hook into dependency injection

Update [VisitzScreens](../../visitz/Visitz/VisitzConfig/VisitzScreens.cs) to register your new view as a type for the DI system to use. 

Prefer to register views as **Transient** unless you have an explicit reason to make it a Singleton.

Make sure your view has a parameterless constructor, or it will not work with DI. This has implications on how to initialize views.

### Initializing your view with data

Because DI only really works with parameterless constructors, you'll need to init the view in other ways. Note that you must use BaseContentView or a derived type for the following behaviour:

#### Inside the view

1. Use the `InitAsync` lifecycle method to read from `public` properties and set up your view with what it needs to work.

1. (If using `ViewModelContentView` or more derived) Your view model will typically need initialization data the most, so outside callers can either directly set properties on the view model (via the `ViewModelContentView.ViewModel` property), or you need to make pass-through properties on your view that set the view model properties directly. You could also make a public `Init` method on your view that accepts named parameters, then it sets the respective view model properties.

#### Outside the view

1. Get a new instance of the view with DI (`ServiceProvider`).

1. Set its `public` properties with information it needs to use (or set its view model's `public` properties, or call a custom init method on the view itself).

1. Add the view to the visual tree (add it to a Grid, StackLayout, etc.).

1. Once added, the view will invoke InitAsync on its own and begin initialization.

1. If you don't need to interact with the view from outside at all, you're done! Otherwise, you can await its `InitTask` to be sure it has finished initializing before interacting with it.

### Override `Dispose(bool)` if necessary

`BaseContentView` and `VisitzViewModel` both implement the IDisposable pattern and it is recommended to override the `Dispose(bool)` function to dispose any resources your derived type uses (like database connections). 

Don't forget to use the same `bool disposed` pattern, as the base types' calls to `Dispose()` won't prevent duplicate disposals of your type.

If you use `VisitzViewModel` for your view model content views, the view model's `Dispose(bool)` will be automatically called.