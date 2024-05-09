Imports System.Text

'Hello, this is Andre Fontenele
'I wrote this on May 8th
'In this project I used an If-Then Statement as well as a For Loop (In both the traditional C iterator and the newer For In methods), and a Subprocedure (called Reset())

Public Class Form1
    'I organized most of the UI into Arrays so that I can create nice clean loops for later on when calculating the subtotal. So it may not seem initially too logical, but (I hope) it pays off when you look at
    'how easy it would be to add/remove menu items or change their prices. In that sense, the first bit of the program is made to be as declarative as possible. Maybe this design decision was a mistake, but it sure is an interesting idea.
    'To see how I use this data look to Calc_Btn_Click() and Reset().

    'This organization system would also make it relatively easy to edit the menu later if needed.
    Public BurgerRadioButtons() As RadioButton
    Public BurgerOptionStrings() As String
    Public BurgerPricing() As Decimal


    Public ToppingCheckBoxes() As CheckBox
    'For toppings we can use the checkbox labels for the strings. Also special about this array is that we do not store the prices in this array directly. We have an array of Catgories which we then transform into an array of prices through a
    'Pricing dictionary in Form1_Load.
    'Again, since this is running right at start-time there shouldn't be any noticable difference for the end user. This is all for developer experience and maintainability/reconfiguration/extensibility.
    Public ToppingCategoryStrings() As String
    Public ToppingPricing() As Decimal


    'For drinks, I decided to give each drink a "price multiplier", which is 1 for every drink except the free water, in which case it is 0
    Public BeverageRadioButtons() As RadioButton
    Public BeverageOptionStrings() As String
    Public BeveragePriceMultipliers() As Decimal

    Public SizeRadioButtons() As RadioButton
    Public SizeOptionStrings() As String
    Public SizePricing() As Decimal


    'This is just to put the value in a more convenient spot in the file
    Public TaxRate As Decimal = 0.05

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Me.BurgerRadioButtons = {Me.BeefRad, Me.TurkeyRad, Me.VeggieRad, Me.BuffaloRad}
        Me.BurgerOptionStrings = {"Beef", "Turkey", "Veggie", "Buffalo"}
        Me.BurgerPricing = {4, 4.25, 5, 6}


        Me.ToppingCheckBoxes = {Me.LettuceCB, Me.TomatoCB, Me.OnionCB, Me.PicklesCB, Me.BaconCB, Me.CheeseCB, Me.GuacCB, Me.KetchupCB, Me.HonMustCB, Me.MayoCB}
        'For toppings we can use the checkbox labels for the strings. Also special about this array is that we do not store the prices in this array directly. We have an array of Catgories which we then transform into an array of prices through a
        'Pricing dictionary in Form1_Load.
        'Again, since this is running right at start-time there shouldn't be any noticable difference for the end user. This is all for developer experience and maintainability/reconfiguration/extensibility.
        Me.ToppingCategoryStrings = {"Veg", "Veg", "Veg", "Veg", "Prem", "Prem", "Prem", "Cond", "Cond", "Cond", "Cond"}
        Me.ToppingPricing = {}


        'For drinks, I decided to give each drink a "price multiplier", which is 1 for every drink except the free water, in which case it is 0
        Me.BeverageRadioButtons = {Me.TeaRad, Me.SodaRad, Me.WaterRad, Me.CoffeeRad, Me.OJRad}
        Me.BeverageOptionStrings = {"Tea", "Soda", "Water", "Coffee", "OJ"}
        Me.BeveragePriceMultipliers = {1, 1, 0, 1, 1}

        Me.SizeRadioButtons = {Me.SmallRad, Me.MedRad, Me.LargeRad}
        Me.SizeOptionStrings = {"Small", "Medium", "Large"}
        Me.SizePricing = {1.5, 2, 2.5}


        'Declare Topping Price Categories
        Dim ToppingCategoryPricing As New Dictionary(Of String, Decimal)
        ToppingCategoryPricing.Add("Veg", 0.1)
        ToppingCategoryPricing.Add("Prem", 1)
        ToppingCategoryPricing.Add("Cond", 0)

        'Populate ToppingPricing Array
        Dim Price As Decimal = -1
        For Each CategoryString In ToppingCategoryStrings
            ToppingCategoryPricing.TryGetValue(CategoryString, Price)
            If Price = -1 Then
                MessageBox.Show("ERROR: Bad Category: " & CategoryString)
            Else
                Me.ToppingPricing.Append(Price)
            End If
        Next

        'There is probably a more DRY way to do this where you can embed extra information via inheritance on RadioButton or something similar, and if I were more familiar with the OOP aspect of VB.NET I would prob come up with a nicer solution.


        'Clears the UI in preperation for user input
        Me.Reset()

    End Sub


    'This subprocedure is the one which clears all of your setting on the UI. You may be wondering why I decided to refactor this into its own subprocedure instead of just putting the code under Reset_Btn_Click. Well, I was having an issue where VS would
    'activate the Beef radio button upon start, despite the fact that I had it explicitly labeled off in the design panel for the form. To fix this problem, I put all the code to
    'reset the UI into this Subprocedure and I call it on both the click of the reset buttong and also on the loading of the form.
    Public Sub Reset()
        For Each RadBut In BurgerRadioButtons
            RadBut.Checked = False
        Next

        For Each ChkBx In ToppingCheckBoxes
            ChkBx.Checked = False
        Next

        For Each RadBut In BeverageRadioButtons
            RadBut.Checked = False
        Next

        For Each RadBut In SizeRadioButtons
            RadBut.Checked = False
        Next
    End Sub

    Private Sub Calc_Btn_Click(sender As Object, e As EventArgs) Handles Calc_Btn.Click
        'These variables are the ones we want to fill as we go
        'RunningSubTotal will eventually be put into the subtotal text field, and the rest will be concatenated into a sort of Reciept text field
        Dim RunningSubTotal As Decimal = 0
        Dim BurgerType As String = ""
        Dim ToppingStrings() As String
        Dim BeverageType As String = ""
        Dim BeverageSize As String = ""
        Dim BeveragePriceMultiplier As Decimal
        Dim BeverageBasePrice As Decimal

        'Add Burger Cost and Type
        For i As Integer = 0 To (Len(Me.BurgerRadioButtons) - 1)
            If Me.BurgerRadioButtons(i).Checked Then
                RunningSubTotal = Me.BurgerPricing(i)
                BurgerType = Me.BurgerOptionStrings(i)
                Exit For
            End If
        Next
        'Throw an error if no Burger Type was entered
        If BurgerType = "" Then
            MessageBox.Show("Please Enter the Type of Burger before calculating costs")
            Exit Sub
        End If


        'Add Topping Cost and Types
        For i As Integer = 0 To Len(Me.ToppingCheckBoxes) - 1
            If Me.ToppingCheckBoxes(i).Checked Then
                RunningSubTotal += Me.ToppingPricing(i)
                'I would do the string concatentation inline here except the logic for figuring out where to put the columns gets complicated
                ToppingStrings.Append(Me.ToppingCheckBoxes(i).Text)
            End If
        Next


        'Add Beverage Type and find BeveragePriceMultiplier
        For i As Integer = 0 To Len(Me.BeverageRadioButtons)
            If Me.BeverageRadioButtons(i).Checked Then
                BeverageType = Me.BeverageOptionStrings(i)
                BeveragePriceMultiplier = Me.BeveragePriceMultipliers(i)
                Exit For
            End If
        Next
        'Throw an error if no BeverageType was entered
        If BeverageType = "" Then
            MessageBox.Show("Please enter the beverage type you would like before calculating costs")
            Exit Sub
        End If

        'Calculate Base Beverage Cost
        For i As Integer = 0 To Len(Me.SizeRadioButtons)
            If Me.SizeRadioButtons(i).Checked Then
                BeverageSize = Me.SizeOptionStrings(i)
                BeverageBasePrice = Me.SizePricing(i)
            End If
        Next
        'Throw an error if no SizeType was entered
        If BeverageType = "" Then
            MessageBox.Show("Please enter the beverage size you would like before calculating costs")
            Exit Sub
        End If

        'Calculate Beverage Cost and add to running subtotal
        RunningSubTotal += BeverageBasePrice * BeveragePriceMultiplier

        'Generate Reciept String
        Dim RecieptString As String
        If Len(ToppingStrings) = 0 Then
            RecieptString = "You ordered a plain " & BurgerType & " Burger"

        Else
            RecieptString = "You ordered a " & BurgerType & " Burger with " & ToppingStrings(0)

            If Len(ToppingStrings) > 1 Then
                For i As Integer = 1 To Len(ToppingStrings) - 1
                    If i = 0 Then
                        RecieptString &= ", " & ToppingStrings(i)
                    End If
                Next
            End If
        End If

        RecieptString &= " with a " & BeverageSize & " " & BeverageType

        'Calculate the Taxes and Total Cost. NOTE: We are not given a spec for the amount to be taxed so I assume that it is ~5% which comes decently close to the example we are given. This is also rounded up to the nearest cent using Math.Ceiling
        Dim Tax As Decimal = Math.Ceiling(RunningSubTotal * Me.TaxRate * 100) / 100

        'Update the UI
        SubTot_TxtBx.Text = RunningSubTotal
        Tax_TxtBx.Text = Tax
        Total_TxtBx.Text = RunningSubTotal + Tax
        RecieptLabel.Text = RecieptString

    End Sub

    Private Sub Reset_Btn_Click(sender As Object, e As EventArgs) Handles Reset_Btn.Click
        Me.Reset()
    End Sub
End Class
