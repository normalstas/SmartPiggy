namespace smartPiggy;

public partial class TaskPage : ContentPage
{
	public TaskPage()
	{
		InitializeComponent();
	}

	private void allbtn_Clicked(object sender, EventArgs e)
	{
		var btn = sender as Button;

		switch (btn.CommandParameter)
		{
			case "add":

				break;
			case "remove":

				break;
			case "delete":

				break;
			case "allcate":

				break;
			case "workcate":

				break;
			case "housecate":

				break;
			case "learncate":

				break;
			case "alsocate":

				break;
			case "complete":

				break;
			case "nocomplete":

				break;
			case "allcomplete":

				break;
			default:
				break;
		}
	}
}