

namespace FantasyFootball.Shared
{
    public class DropdownModel
    {
        //
        // Summary:
        //     Gets or sets a value that indicates whether this Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        //     is disabled. This property is typically rendered as a disabled="disabled" attribute
        //     in the HTML <option> element.
        public bool Disabled { get; set; }
        //
        // Summary:
        //     Gets or sets a value that indicates whether this Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        //     is selected. This property is typically rendered as a selected="selected" attribute
        //     in the HTML <option> element.
        public bool Selected { get; set; }
        //
        // Summary:
        //     Gets or sets a value that indicates the display text of this Microsoft.AspNetCore.Mvc.Rendering.SelectListItem.
        //     This property is typically rendered as the inner HTML in the HTML <option> element.
        public string Text { get; set; }
        //
        // Summary:
        //     Gets or sets a value that indicates the value of this Microsoft.AspNetCore.Mvc.Rendering.SelectListItem.
        //     This property is typically rendered as a value="..." attribute in the HTML <option>
        //     element.
        public string Value { get; set; }
    }
}
