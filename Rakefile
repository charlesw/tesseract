require 'bundler/setup'

require 'albacore'
# require 'albacore/tasks/release'
require 'albacore/tasks/versionizer'
require 'albacore/ext/teamcity'

Configuration = ENV['CONFIGURATION'] || 'Release45'

Albacore::Tasks::Versionizer.new :versioning

desc 'create assembly infos'
asmver :assembly_info do |a|
  a.file_path = './src/AssemblyVersionInfo.cs'
  a.namespace = 'Tesseract'
  a.attributes assembly_version: ENV['LONG_VERSION'],
               assembly_file_version: ENV['LONG_VERSION'],
               assembly_informational_version: ENV['BUILD_VERSION']
end

desc 'Perform fast build (warn: doesn\'t d/l deps)'
build :quick_compile do |b|
  b.prop 'Configuration', Configuration
  b.logging = 'detailed'
  b.sln     = 'src/Tesseract.sln'
end

task :paket_bootstrap do
  system 'tools/paket.bootstrapper.exe', clr_command: true unless   File.exists? 'tools/paket.exe'
end

desc 'restore all nugets as per the packages.config files'
task :restore => :paket_bootstrap do
  system 'tools/paket.exe', 'restore', clr_command: true
end

desc 'Perform full build'
build :compile => [:versioning, :restore, :assembly_info] do |b|
  b.prop 'Configuration', Configuration
  b.sln = 'src/Tesseract.sln'
end

directory 'build/pkg'

desc 'package nugets - finds all projects and package them'
nugets_pack :create_nugets => ['build/pkg', :versioning, :compile] do |p|
  p.configuration = Configuration
  p.files   = FileList['src/**/*.{csproj,fsproj,nuspec}'].
    exclude(/Tests/)
  p.out     = 'build/pkg'
  p.exe     = 'packages/NuGet.CommandLine/tools/NuGet.exe'
  p.with_metadata do |m|
    # m.id          = 'MyProj'
    m.title       = 'TODO'
    m.description = 'TODO'
    m.authors     = 'John Doe, Foretag AB'
    m.project_url = 'http://example.com'
    m.tags        = ''
    m.version     = ENV['NUGET_VERSION']
  end
end

namespace :tests do
  task :unit do
    system "packages/NUnit.Runners/tools/nunit-console.exe",
           %w|src/Tesseract.Tests/bin/Release/Net45/Tesseract.Tests.dll|,
           clr_command: true
  end
end

task :tests => :'tests:unit'

task :default => [:create_nugets ,:tests]

#task :ensure_nuget_key do
#  raise 'missing env NUGET_KEY value' unless ENV['NUGET_KEY']
#end

#Albacore::Tasks::Release.new :release,
#                             pkg_dir: 'build/pkg',
#                             depend_on: [:create_nugets, :ensure_nuget_key],
#                             nuget_exe: 'packages/NuGet.CommandLine/tools/NuGet.exe',
#                             api_key: ENV['NUGET_KEY']
