package icons

import com.intellij.icons.AllIcons
import com.intellij.openapi.util.IconLoader
import com.intellij.ui.LayeredIcon
import org.jetbrains.annotations.NotNull
import javax.swing.Icon
import javax.swing.SwingConstants

class VerifyIcons {
    companion object {
        // Themed icon by naming convention (XXX_dark)
        // https://jetbrains.design/intellij/resources/icons_list/
        @JvmField val CompareIcon = IconLoader.getIcon("/icons/compare.svg", VerifyIcons::class.java)
        @JvmField val AcceptIcon = IconLoader.getIcon("/icons/accept.svg", VerifyIcons::class.java)
    }
}