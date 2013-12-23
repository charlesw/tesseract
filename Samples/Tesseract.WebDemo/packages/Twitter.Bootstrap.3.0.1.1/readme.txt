Twitter.Bootstrap is moving!

v3.0.1 will be the last release of Bootstrap under the Twitter.Bootstrap namespace. With new projects moving forward, you should install Bootstrap directly from the Bootstrap namespace maintained by Outercurve https://www.nuget.org/packages/Bootstrap/
For existing projects, you already have the new Bootstrap package installed by upgrading to this release.

After Migration the folder structure of the bootstrap files will be as follows
- Application Root
- Content
---- css files such as bootstrap.css
- fonts
---- all fonts files
- Scripts
---- all script files such as bootstrap.js

This move does not affect the Twitter.Bootstrap.MVC package or the Twitter.Bootstrap.Less package. Those packages will continue to be maintained with each new bootstrap release. Visit github (https://github.com/sirkirby/twitter-bootstrap-nuget) for more info.

Any questions or issues? Ask me on twitter @sirkirby or visit http://chriskirby.net/bootstrap-nuget-package-moving-to-outercurve/