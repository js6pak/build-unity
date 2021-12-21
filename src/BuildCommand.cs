using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;

namespace kuler90
{
    public static class BuildCommand
    {
        private static string originalDefines;

        public static void Build()
        {
            var args = ParseCommandLineArguments();

            var target = HandleTarget(args);
            var buildPath = HandleBuildPath(args);
            var scenes = HandleScenesList(args);

            if (args.ContainsKey("scriptingBackend"))
            {
                PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, (ScriptingImplementation)Enum.Parse(typeof(ScriptingImplementation), args["scriptingBackend"], true));
            }

            var buildReport = BuildPipeline.BuildPlayer(scenes, buildPath, target, BuildOptions.None);
            int code = (buildReport.summary.result == BuildResult.Succeeded) ? 0 : 1;

            EditorApplication.Exit(code);
        }

        private static BuildTarget HandleTarget(Dictionary<string, string> args)
        {
            var target = (BuildTarget)Enum.Parse(typeof(BuildTarget), args["buildTarget"], true);
            return target;
        }

        private static string HandleBuildPath(Dictionary<string, string> args)
        {
            var buildPath = args["buildPath"];
            return buildPath;
        }

        private static string[] HandleScenesList(Dictionary<string, string> args)
        {
            return EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(s => s.path).ToArray();
        }

        private static Dictionary<string, string> ParseCommandLineArguments()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            string[] args = Environment.GetCommandLineArgs();

            // Extract flags with optional values
            for (int current = 0, next = 1; current < args.Length; current++, next++)
            {

                // Parse flag
                bool isFlag = args[current].StartsWith("-");
                if (!isFlag) continue;
                string flag = args[current].TrimStart('-');

                // Parse optional value
                bool flagHasValue = next < args.Length && !args[next].StartsWith("-");
                string value = flagHasValue ? args[next].TrimStart('-') : "";

                result.Add(flag, value);
            }

            return result;
        }
    }
}