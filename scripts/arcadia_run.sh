echo "I have no idea what I'm doing."
root=$(pwd)

if hash dotnet 1>/dev/null 2>&1
then
	echo "Found .NET SDK."
else
	echo ".NET SDK 3.1 is missing. Please run arcadia_install_preq.sh."
	exit 1
fi

cd "$root/Orikivo"
dotnet restore
echo "Restored Arcadia."
dotnet build --configuration Release

# Copy configuration file over to the build results
cd "$root"
cp -f "$root/Orikivo/Arcadia/config.json" "$root/Orikivo/Arcadia/bin/Release/netcoreapp3.1" 1>/dev/null 2>&1
echo "Resolved config.json and placed in build directory."

cd "$root/Orikivo/Arcadia"

echo "Starting Arcadia. Please wait..."
dotnet run --configuration Release
echo "Done!"

cd "$root"
rm "$root/arcadia_run.sh"
exit 0