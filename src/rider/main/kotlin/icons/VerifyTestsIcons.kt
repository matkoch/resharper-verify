package icons

import com.intellij.icons.AllIcons
import com.intellij.openapi.util.IconLoader
import com.intellij.ui.LayeredIcon
import org.jetbrains.annotations.NotNull
import javax.swing.Icon
import javax.swing.SwingConstants

class VerifyTestsIcons {
    companion object {
        // Themed icon by naming convention (XXX_dark)
        // https://jetbrains.design/intellij/resources/icons_list/
        @JvmField val Icon = IconLoader.getIcon("/META-INF/pluginIcon.svg")
        @JvmField val CompareIcon = IconLoader.getIcon("/icons/compare.svg")
        @JvmField val AcceptIcon = IconLoader.getIcon("/icons/accept.svg")

        private fun load(attachment: Icon): @NotNull Icon {
            val icon = LayeredIcon(2)
            icon.setIcon(Icon, 0)
            icon.setIcon(attachment, 1)
            return icon
        }
    }
}