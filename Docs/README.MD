SSR.Net
============

SSR.Net makes it possible to server side render components in React 17, React 18 and Vue 3. It is open for extension, and can support other frameworks.

## Installation
This library can be installed through NuGet. 


Limitations when doing SSR:
* The window object does not exist. This means that your code needs to check if window is defined before using it:
if (typeof window !== 'undefined')
{
//Do stuff with window here
}
* Global variables must not be used. Each JavaScript Engine is used multiple times, so global variables will cause state to be shared between requests, meaning data can leak from one user to another.
