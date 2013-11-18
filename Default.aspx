<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="_Default" Debug="true" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <asp:TextBox ID="tbQuery" runat="server" Width="226px" Text="#MongoDB"></asp:TextBox>
    <asp:Button ID="btnSerachTweets" runat="server" Text="Search Tweets" OnClick="btnSearchTweets_Click" />
    <asp:Button ID="btnGetTweetsFromDB" runat="server" Text="Get Tweets From DB" OnClick="btnGetTweetsFromDB_Click" />
    <br />
    <br />
    <asp:Label ID="lblStatistics" runat="server" Text="Statistics: ..." Style="color: Red;
        font-size: medium;"></asp:Label>
    <br />
    <asp:GridView ID="gvTweetsFromDB" runat="server" AutoGenerateColumns="true">
    </asp:GridView>
    <br />
</asp:Content>
