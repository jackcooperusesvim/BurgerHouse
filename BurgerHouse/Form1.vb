Imports System.Linq.Expressions
Imports System.Text

'Hello, this is Andre Fontenele
'I wrote this on May 8th
'In this project I used If-Then Statement, as well as both types of For Loop , as well as some Select Case Statements, as well as a Subprocedure and a Function 

Public Class Form1
    'I organized most of the UI into Arrays so that I can create nice clean loops for later on when calculating the subtotal. So it may not seem initially too logical, but (I hope) it pays off when you look at
    'how easy it would be to add/remove menu items or change their prices. In that sense, the first bit of the program is made to be as declarative as possible. Maybe this design decision was a mistake, but it sure is an interesting idea.
    'To see how I use this data look to Calc_Btn_Click() and Reset().

    'This organization system would also make it relatively easy to edit the menu later if needed.
    Public NumBurgerTypes As Integer = 4
    Public BurgerRadioButtons(NumBurgerTypes - 1) As RadioButton
    Public BurgerOptionStrings(NumBurgerTypes - 1) As String
    Public BurgerPricing(NumBurgerTypes - 1) As Decimal


    Public NumToppingTypes As Integer = 10
    Public ToppingCheckBoxes(NumToppingTypes - 1) As CheckBox
    'For toppings we can use the checkbox labels for the strings. Also special about this array is that we do not store the prices in this array directly. We have an array of Catgories which we then transform into an array of prices through a
    'Pricing dictionary in Form1_Load.
    'Again, since this is running right at start-time there shouldn't be any noticable difference for the end user. This is all for developer experience and maintainability/reconfiguration/extensibility.
    Public ToppingCategoryStrings(NumToppingTypes - 1) As String
    Public ToppingPricing(NumToppingTypes - 1) As Decimal


    'For drinks, I decided to give each drink a "price multiplier", which is 1 for every drink except the free water, in which case it is 0
    Public NumBeverageTypes As Integer = 5
    Public BeverageRadioButtons(NumBeverageTypes - 1) As RadioButton
    Public BeverageOptionStrings(NumBeverageTypes - 1) As String
    Public BeveragePriceMultipliers(NumBeverageTypes - 1) As Decimal

    Public NumSizeTypes As Integer = 3
    Public SizeRadioButtons(NumSizeTypes - 1) As RadioButton
    Public SizeOptionStrings(NumSizeTypes - 1) As String
    Public SizePricing(NumSizeTypes - 1) As Decimal


    'This is just to put the value in a more convenient spot in the file
    Public TaxRate As Decimal = 0.05

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Me.BurgerRadioButtons = {Me.BeefRad, Me.TurkeyRad, Me.VeggieRad, Me.BuffaloRad}
        Me.BurgerOptionStrings = {"beef", "turkey", "veggie", "buffalo"}
        Me.BurgerPricing = {4, 4.25, 5, 6}


        Me.ToppingCheckBoxes = {Me.LettuceCB, Me.TomatoCB, Me.OnionCB, Me.PicklesCB, Me.BaconCB, Me.CheeseCB, Me.GuacCB, Me.KetchupCB, Me.HonMustCB, Me.MayoCB}
        'For toppings we can use the checkbox labels for the strings. Also special about this array is that we do not store the prices in this array directly. We have an array of Catgories which we then transform into an array of prices through a
        'Pricing dictionary in Form1_Load.
        'Again, since this is running right at start-time there shouldn't be any noticable difference for the end user. This is all for developer experience and maintainability/reconfiguration/extensibility.
        Me.ToppingCategoryStrings = {"Veg", "Veg", "Veg", "Veg", "Prem", "Prem", "Prem", "Cond", "Cond", "Cond", "Cond"}


        'For drinks, I decided to give each drink a "price multiplier", which is 1 for every drink except the free water, in which case it is 0
        Me.BeverageRadioButtons = {Me.TeaRad, Me.SodaRad, Me.WaterRad, Me.CoffeeRad, Me.OJRad}
        Me.BeverageOptionStrings = {"tea", "soda", "water", "voffee", "OJ"}
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
        Dim Price As Decimal
        For i As Integer = 0 To NumToppingTypes - 1
            ToppingCategoryPricing.TryGetValue(ToppingCategoryStrings(i), Price)
            Me.ToppingPricing(i) = Price
        Next

        'There is probably a more DRY way to do this where you can embed extra information via inheritance on RadioButton or something similar, and if I were more familiar with the OOP aspect of VB.NET I would prob come up with a nicer solution.


        'This line was intended to clear the UI in preperation for user input, but apparently sometime just after loading, the
        'Beef Radio button is automatically checked. The only solution to this would probably set up Me.Reset() to run Asynchronously
        'and add a small time delay at the beginning of the function
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
        Dim NumToppingsOnBurger As Integer = 0
        Dim ToppingStrings(NumToppingTypes) As String
        Dim BeverageType As String = ""
        Dim BeverageSize As String = ""
        Dim BeveragePriceMultiplier As Decimal
        Dim BeverageBasePrice As Decimal

        'Add Burger Cost and Type
        For i As Integer = 0 To Me.NumBurgerTypes - 1
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
        For i As Integer = 0 To Me.NumToppingTypes - 1
            If Me.ToppingCheckBoxes(i).Checked Then
                RunningSubTotal += Me.ToppingPricing(i)
                'I would do the string concatentation inline here except the logic for figuring out where to put the columns gets complicated
                ToppingStrings(NumToppingsOnBurger) = Me.ToppingCheckBoxes(i).Text
                NumToppingsOnBurger += 1
            End If
        Next


        'Add Beverage Type and find BeveragePriceMultiplier
        For i As Integer = 0 To Me.NumBeverageTypes - 1
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
        For i As Integer = 0 To Me.NumSizeTypes - 1
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


        'Create final ReciptString
        Dim RecieptString As String
        'Add Burger To Recipt
        If ToppingStrings(0) = "" Then
            RecieptString = "You ordered a plain " & BurgerType & " Burger"
        Else
            RecieptString = "You ordered a " & BurgerType & " Burger"
        End If

        'Add Toppings To Recipt
        RecieptString &= GenerateToppingString(ToppingStrings, NumToppingsOnBurger)
        'Add Beverage To Recipt
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

    ' I put a lot of work into this next function, so please do take a look. What this function does is it turns a array of toppings (and a number for indexing the end of the array) into a formatted string using all (hopefully) the proper grammer
    'eg:
    '   ... Burger ...
    '   ... Burger with Guac ...
    '   ... Burger with Guac and Mayo ...
    '   ... Burger with Guac, Mayo, Cheese, and Pickles ...
    ' These 4 examples above also each each correspond with one Case in the Select Case Statement below
    Function GenerateToppingString(ByVal ToppingStringsOnBurger() As String, NumToppings As Integer) As String
        Select Case NumToppings
            Case 0
                Return ""
            Case 1
                Return " with " & ToppingStringsOnBurger(0)
            Case 2
                Return " with " & ToppingStringsOnBurger(0) & " and " & ToppingStringsOnBurger(1)
            Case Else
                Dim ToppingFormattedString As String = " with "
                For i As Integer = 0 To NumToppings - 1
                    Select Case i
                        Case 0
                            ToppingFormattedString &= LCase(ToppingStringsOnBurger(i))
                        Case NumToppings - 1
                            ToppingFormattedString &= ", and "
                            ToppingFormattedString &= LCase(ToppingStringsOnBurger(i))
                        Case Else
                            ToppingFormattedString &= ", "
                            ToppingFormattedString &= LCase(ToppingStringsOnBurger(i))
                    End Select
                Next
                Return ToppingFormattedString
        End Select
    End Function

    Private Sub Exit_Btn_Click(sender As Object, e As EventArgs) Handles Exit_Btn.Click
        Me.Close()
    End Sub
End Class
