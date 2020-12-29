# This installs all pre-requisites needed to host Arcadia

root=$(pwd)
echo "Preparing for installiation..."
sudo add-apt-repository universe
sudo apt-get install software-properties-common apt-transport-https curl gpg -y
wget -qO- https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > microsoft.asc.gpg
sudo mv -f microsoft.asc.gpg /etc/apt/trusted.gpg.d/
wget -q https://packages.microsoft.com/config/ubuntu/18.04/prod.list 
sudo mv -f prod.list /etc/apt/sources.list.d/microsoft-prod.list
sudo chown root:root /etc/apt/trusted.gpg.d/microsoft.asc.gpg
sudo chown root:root /etc/apt/sources.list.d/microsoft-prod.list
sudo apt-get update
sudo apt-get upgrade -y
echo "Installing Git..."
sudo apt-get install git -y
echo "Installing .NET Core SDK 3.1..."
sudo apt-get install dotnet-sdk-3.1 -y

cd "$root"
rm "$root/arcadia_install_preq.sh"
exit 0