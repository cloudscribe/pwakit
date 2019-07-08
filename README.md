# pwakit
aiming to provide tools to facilitate building PWAs (Progressive Web Apps) with ASP.NET Core

## Features

Configurable dynamic runtime creation of [serviceworker](https://developers.google.com/web/ilt/pwa/introduction-to-service-worker) using [google workbox](https://developers.google.com/web/tools/workbox/) to make your web app work offline.

Push notifications - on supported browsers and devices push notifications can be used for both visible notifications and non-visible notifications sent to the serviceworker, for example to instruct the serviceworker to add, update or delete update cached content.

## Try It

You can log into the demo site in this repository using username admin and password admin.

You can then opt in to push notifications and under Administration > Push Notification you can send notifications for testing purposes.

Documentation coming soon.

## Credits

Some of the work on serviceworkers was informed by Mads Kristensen's [WebEssentials.AspNetCore.ServiceWroeker](https://github.com/madskristensen/WebEssentials.AspNetCore.ServiceWorker)

The work on push notifications was very informed by the work of Tomasz Pęczek in his [demo app here](https://github.com/tpeczek/Demo.AspNetCore.PushNotifications), and we are using his library [Lib.Net.Http.WebPush](https://github.com/tpeczek/Lib.Net.Http.WebPush)


## Sponsors

![esdm logo](https://www.cloudscribe.com/media/images/esdm-banner.png)

The initial code in this solution was developed under sponsored open source development with funding provided by [exeGesIS Spatial Data Management](https://www.esdm.co.uk/)

If you are interested in sponsoring additional open source features for the cloudscribe ecosystem, please [contact us](https://www.cloudscribe.com/contact)

See the [complete list of cloudscribe libraries](https://www.cloudscribe.com/docs/complete-list-of-cloudscribe-libraries) on [cloudscribe.com](https://www.cloudscribe.com/)

## Questions or Comments?

[![Join the chat at https://gitter.im/joeaudette/cloudscribe](https://badges.gitter.im/joeaudette/cloudscribe.svg)](https://gitter.im/joeaudette/cloudscribe?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
