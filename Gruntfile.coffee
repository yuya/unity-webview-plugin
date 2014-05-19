matchdep = require "matchdep"

module.exports = (grunt) ->
  basepath = grunt.file.readJSON("filepath.json")
  filepath =
    cs     : basepath.cs
    mm     : basepath.mm
    jar    : basepath.jar
    bundle : basepath.bundle
    xcode  : basepath.xcode
  config =
    pkg: grunt.file.readJSON "package.json"

    exec:
      cp:
        cmd: (from, to) ->
          return "cp -rf #{from} #{to}"

    esteWatch:
      options:
        dirs: [
          "build/Packager/Assets/Plugins/**/"
          "plugins/**/"
        ]
        livereload:
          enabled: false

      cs: (file) ->
        return ["exec:cp:#{file}:#{filepath.cs}"]

      mm: (file) ->
        task = [
          "exec:cp:#{file}:#{filepath.mm}"
          "exec:cp:#{file}:#{filepath.xcode.mm}"
        ]

        return task

      jar: (file) ->
        return ["exec:cp:#{file}:#{filepath.jar}"]

      bundle: (file) ->
        return ["exec:cp:#{file}:#{filepath.bundle}"]

  grunt.initConfig config
  matchdep.filterDev("grunt-*").forEach grunt.loadNpmTasks

  grunt.registerTask "default", ["esteWatch"]
  grunt.registerTask "watch", "watching \.(cs|mm|jar|bundle) files.", ->
    console.log "environment : %s", opts.env
    console.log "livereload  : %s", opts.livereload
    grunt.task.run "esteWatch"
