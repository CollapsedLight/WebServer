using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Routing;

namespace CollapsedLight.WebApp.Components
{
    public class ClNavLinkTemplate : ComponentBase, IDisposable
    {
        private const string DefaultActiveClass = "active";

        private bool _isActive;
        private string _linkAbsolute = string.Empty;
        private string? _class;

        /// <summary>
        /// Gets or sets the CSS class name applied to the NavLink when the
        /// current route matches the NavLink href.
        /// </summary>
        [Parameter]
        public string? ActiveClass { get; set; }

        [Parameter]
        public string? InActiveClass { get; set; }

        /// <summary>
        /// Gets or sets a collection of additional attributes that will be added to the generated
        /// <c>a</c> element.
        /// </summary>
        [Parameter(CaptureUnmatchedValues = true)]
        public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

        /// <summary>
        /// Gets or sets the computed CSS class based on whether or not the link is active.
        /// </summary>
        protected string? CssClass { get; set; }

        /// <summary>
        /// Gets or sets the child content of the component.
        /// </summary>
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Gets or sets a value representing the URL matching behavior.
        /// </summary>
        [Parameter]
        public NavLinkMatch Match { get; set; }

        /// <summary>
        ///     Link navigated to when clicked.
        /// </summary>
        [Parameter]
        public string? Link { get; set; }

        [Inject]
        private NavigationManager NavigationManger { get; set; } = default!;

        /// <inheritdoc />
        protected override void OnInitialized()
        {
            // We'll consider re-rendering on each location change
            NavigationManger.LocationChanged += OnLocationChanged;
        }

        /// <inheritdoc />
        protected override void OnParametersSet()
        {
            if (Link == null)
            {
                if (AdditionalAttributes != null && AdditionalAttributes.TryGetValue("href", out var link))
                    Link = Convert.ToString(link);
                else
                    throw new InvalidOperationException($"The {nameof(ClNavLinkTemplate)} component requires a value for the parameter {nameof(Link)} or the HTML attribute href");
            }

            _linkAbsolute = NavigationManger.ToAbsoluteUri(Link).AbsoluteUri;
            _isActive = ShouldMatch(NavigationManger.Uri);

            _class = null;
            if (AdditionalAttributes != null && AdditionalAttributes.TryGetValue("class", out var obj))
                _class = Convert.ToString(obj);

            UpdateCssClass();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // To avoid leaking memory, it's important to detach any event handlers in Dispose()
            NavigationManger.LocationChanged -= OnLocationChanged;
        }

        private void UpdateCssClass()
        {
            var styling = _isActive ? ActiveClass : InActiveClass;
            styling ??= DefaultActiveClass;
            CssClass = CombineWithSpace(_class, styling);
        }

        private void OnLocationChanged(object? sender, LocationChangedEventArgs args)
        {
            // We could just re-render always, but for this component we know the
            // only relevant state change is to the _isActive property.
            var shouldBeActiveNow = ShouldMatch(args.Location);
            if (shouldBeActiveNow != _isActive)
            {
                _isActive = shouldBeActiveNow;
                UpdateCssClass();
                StateHasChanged();
            }
        }

        private bool ShouldMatch(string currentUriAbsolute)
        {
            if (Match == NavLinkMatch.Prefix
             && IsStrictlyPrefixWithSeparator(currentUriAbsolute, _linkAbsolute))
                return true;

            return LinkEqualsExactlyOrIfTrailingSlashAdded(currentUriAbsolute);
        }

        private bool LinkEqualsExactlyOrIfTrailingSlashAdded(string currentUriAbsolute)
        {
            if (string.Equals(currentUriAbsolute, _linkAbsolute, StringComparison.OrdinalIgnoreCase))
                return true;

            if (currentUriAbsolute.Length != _linkAbsolute.Length - 1)
                return false;

            // Special case: highlight links to http://host/path/ even if you're
            // at http://host/path (with no trailing slash)
            //
            // This is because the router accepts an absolute URI value of "same
            // as base URI but without trailing slash" as equivalent to "base URI",
            // which in turn is because it's common for servers to return the same page
            // for http://host/vdir as they do for host://host/vdir/ as it's no
            // good to display a blank page in that case.
            return _linkAbsolute[^1] == '/'
                && _linkAbsolute.StartsWith(currentUriAbsolute, StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc/>
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "a");

            builder.AddMultipleAttributes(1, AdditionalAttributes);
            builder.AddAttribute(2, "class", CssClass);
            builder.AddAttribute(3, "href", Link);
            builder.AddContent(4, ChildContent);

            builder.CloseElement();
        }

        private string? CombineWithSpace(string? str1, string str2)
            => str1 == null ? str2 : $"{str1} {str2}";

        private static bool IsStrictlyPrefixWithSeparator(string value, string prefix)
        {
            var prefixLength = prefix.Length;
            if (value.Length > prefixLength)
            {
                return value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
                    && (
                           // Only match when there's a separator character either at the end of the
                           // prefix or right after it.
                           // Example: "/abc" is treated as a prefix of "/abc/def" but not "/abcdef"
                           // Example: "/abc/" is treated as a prefix of "/abc/def" but not "/abcdef"
                           prefixLength == 0
                        || !char.IsLetterOrDigit(prefix[prefixLength - 1])
                        || !char.IsLetterOrDigit(value[prefixLength])
                       );
            }

            return false;
        }
    }
}