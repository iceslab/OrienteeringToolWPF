namespace OrienteeringToolWPF.Windows.Forms
{
    interface IForm
    {
        void ObjectToForm();
        ErrorList FormToObject();
        ErrorList ValidateForm();
    }
}
